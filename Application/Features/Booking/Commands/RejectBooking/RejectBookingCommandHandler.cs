using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.Commands.RejectBooking
{
    public class RejectBookingCommandHandler
        : IRequestHandler<RejectBookingCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly ILogger<RejectBookingCommandHandler> _logger;

        public RejectBookingCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGateway,
            ILogger<RejectBookingCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task<Result> Handle(
            RejectBookingCommand request,
            CancellationToken cancellationToken)
        {
            if (_currentUser.WorkshopId is not int workshopId)
                return Result.Failure("Current user is not a workshop.");

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var booking = await _unitOfWork.Bookings
                    .GetByIdWithDetailsAsync(
                        request.BookingId,
                        cancellationToken);

                if (booking is null)
                    return Result.Failure("Booking not found.");

                if (booking.WorkshopId != workshopId)
                    return Result.Failure(
                        "You are not authorized to reject this booking.");

                var alreadyRegistered = await _unitOfWork.WorkshopMonthlyActions
                    .ExistsAsync(
                        workshopId,
                        booking.Id,
                        WorkshopActionType.ManualRejection,
                        cancellationToken);

                if (alreadyRegistered)
                    return Result.Failure(
                        "This rejection action has already been registered.");

                var now = DateTime.UtcNow;

                var monthlyLimit =
                    await _unitOfWork.WorkshopMonthlyActionLimits
                        .GetByWorkshopAndPeriodAsync(
                            workshopId,
                            now.Year,
                            now.Month,
                            cancellationToken);

                if (monthlyLimit is null)
                {
                    monthlyLimit = WorkshopMonthlyActionLimit.Create(
                        workshopId,
                        now.Year,
                        now.Month);

                    await _unitOfWork.WorkshopMonthlyActionLimits
                        .AddAsync(monthlyLimit, cancellationToken);
                }

                var decision = monthlyLimit.RegisterAction(
                    booking.ConfirmationFeeAmount);

                // The Domain method handles rejection and any penalty fee.
                // Its returned authorization ID is used for voiding, if present.
                var authorizationId = booking.RejectByWorkshop(
                    request.Reason,
                    decision);

                var action = WorkshopMonthlyAction.Create(
                    workshopId,
                    booking.Id,
                    now.Year,
                    now.Month,
                    WorkshopActionType.ManualRejection,
                    decision.PenaltyAmount);

                await _unitOfWork.WorkshopMonthlyActions
                    .AddAsync(action, cancellationToken);

                await _unitOfWork.WorkshopMonthlyActionLimits
                    .UpdateAsync(monthlyLimit, cancellationToken);

                // RejectByWorkshop returns an authorization ID when an
                // authorized online payment needs to be voided.
                if (!string.IsNullOrWhiteSpace(authorizationId))
                {
                    var voidResult = await _paymentGateway.VoidAsync(
                        authorizationId,
                        cancellationToken);

                    if (!voidResult.IsSuccess)
                    {
                        _logger.LogError(
                            "Void failed for booking {BookingId}: {Error}",
                            booking.Id,
                            voidResult.ErrorMessage);

                        await transaction.RollbackAsync(cancellationToken);

                        return Result.Failure(
                            "The payment authorization could not be voided. " +
                            "The rejection was not committed; payment status " +
                            "must be checked before retrying.");
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Result.Success(
                    decision.PenaltyApplied
                        ? $"Booking rejected. Workshop penalty: {decision.PenaltyAmount}."
                        : "Booking rejected successfully.");
            }
            catch (DomainException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Error rejecting booking {BookingId}",
                    request.BookingId);

                return Result.Failure(
                    "An unexpected error occurred while rejecting the booking.");
            }
        }
    }
}