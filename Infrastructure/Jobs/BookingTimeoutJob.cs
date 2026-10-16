
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jobs;

public sealed class BookingTimeoutJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingTimeoutJob> _logger;

    public BookingTimeoutJob(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingTimeoutJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredBookingsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error while processing expired bookings.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessExpiredBookingsAsync(
        CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var unitOfWork =
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var paymentGateway =
            scope.ServiceProvider.GetRequiredService<IPaymentGatewayService>();

        var expiredIds = await unitOfWork.Bookings.GetAll()
            .AsNoTracking()
            .Where(b =>
                b.BookingStatus == BookingStatus.PendingApproval &&
                b.ResponseDeadline.HasValue &&
                b.ResponseDeadline.Value <= DateTime.UtcNow)
            .Select(b => b.Id)
            .ToListAsync(ct);

        foreach (var bookingId in expiredIds)
        {
            if (ct.IsCancellationRequested)
                break;

            await ProcessBookingAsync(
                bookingId, unitOfWork, paymentGateway, ct);
        }
    }

    private async Task ProcessBookingAsync(
        int bookingId,
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGateway,
        CancellationToken ct)
    {
        await using var transaction =
            await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var booking = await unitOfWork.Bookings
                .GetByIdWithDetailsAsync(bookingId, ct);

            // Re-check because the workshop may have responded
            // after the initial query.
            if (booking is null ||
                booking.BookingStatus != BookingStatus.PendingApproval ||
                booking.ResponseDeadline is null ||
                booking.ResponseDeadline.Value > DateTime.UtcNow)
            {
                await transaction.RollbackAsync(ct);
                return;
            }

            var now = DateTime.UtcNow;

            var alreadyRegistered =
                await unitOfWork.WorkshopMonthlyActions.ExistsAsync(
                    booking.WorkshopId,
                    booking.Id,
                    WorkshopActionType.TimeoutRejection,
                    ct);

            if (alreadyRegistered)
            {
                _logger.LogWarning(
                    "Timeout action already registered for booking {BookingId}",
                    booking.Id);

                await transaction.RollbackAsync(ct);
                return;
            }

            var monthlyLimit =
                await unitOfWork.WorkshopMonthlyActionLimits
                    .GetByWorkshopAndPeriodAsync(
                        booking.WorkshopId,
                        now.Year,
                        now.Month,
                        ct);

            if (monthlyLimit is null)
            {
                monthlyLimit = WorkshopMonthlyActionLimit.Create(
                    booking.WorkshopId,
                    now.Year,
                    now.Month);

                await unitOfWork.WorkshopMonthlyActionLimits
                    .AddAsync(monthlyLimit, ct);
            }

            var decision = monthlyLimit.RegisterAction(
                booking.ConfirmationFeeAmount);

            var authorizationId =
                booking.AutoRejectDueToTimeout(decision);

            var action = WorkshopMonthlyAction.Create(
                booking.WorkshopId,
                booking.Id,
                now.Year,
                now.Month,
                WorkshopActionType.TimeoutRejection,
                decision.PenaltyAmount);

            await unitOfWork.WorkshopMonthlyActions.AddAsync(action, ct);

            await unitOfWork.WorkshopMonthlyActionLimits
                .UpdateAsync(monthlyLimit, ct);

            // Keep the same payment handling approach as the manual
            // rejection handler. See the important warning below.
            if (!string.IsNullOrWhiteSpace(authorizationId))
            {
                var voidResult = await paymentGateway.VoidAsync(
                    authorizationId, ct);

                if (!voidResult.IsSuccess)
                {
                    _logger.LogError(
                        "Authorization void failed for booking {BookingId}: {Error}",
                        booking.Id,
                        voidResult.ErrorMessage);

                    await transaction.RollbackAsync(ct);
                    return;
                }

                booking.MarkVoided(authorizationId);
            }

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Booking {BookingNumber} auto-rejected. " +
                "Action number: {ActionNumber}. Penalty: {PenaltyAmount}",
                booking.BookingNumber,
                decision.ActionNumber,
                decision.PenaltyAmount);
        }
        catch (DomainException ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogWarning(
                ex, "Could not auto-reject booking {BookingId}", bookingId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(
                ex, "Failed to process expired booking {BookingId}", bookingId);
        }
    }
}
