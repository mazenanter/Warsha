
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.Commands.AcceptBooking;

public sealed class AcceptBookingCommandHandler
    : IRequestHandler<AcceptBookingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly ILogger<AcceptBookingCommandHandler> _logger;

    public AcceptBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IPaymentGatewayService paymentGateway,
        ILogger<AcceptBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _paymentGateway = paymentGateway;
        _logger = logger;
    }

    public async Task<Result> Handle(
        AcceptBookingCommand command,
        CancellationToken ct)
    {
        // 1. Identify the current workshop.
        if (_currentUser.WorkshopId is not int workshopId)
        {
            return Result.Failure(
                "Current user is not associated with a workshop.");
        }

        // 2. Load the booking.
        var booking = await _unitOfWork.Bookings
            .GetByIdWithDetailsAsync(command.BookingId, ct);

        if (booking is null)
            return Result.Failure("Booking not found.");

        // 3. Verify booking ownership.
        if (booking.WorkshopId != workshopId)
            return Result.Failure("You are not authorized to accept this booking.");

        // 4. Apply the domain transition and validate the deadline.
        string? authorizationId;

        try
        {
            authorizationId = booking.AcceptByWorkshop();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        // 5. Persist before performing an external payment operation.
        await _unitOfWork.SaveChangesAsync(ct);

        // Cash is confirmed and its fees are registered
        // inside AcceptByWorkshop().
        if (booking.PaymentType == PaymentType.CashOnDelivery)
        {
            _logger.LogInformation(
                "Cash booking {BookingNumber} accepted by workshop {WorkshopId}.",
                booking.BookingNumber,
                workshopId);

            return Result.Success("Booking accepted successfully.");
        }

        // 6. Online bookings require an authorization ID.
        if (string.IsNullOrWhiteSpace(authorizationId))
        {
            _logger.LogError(
                "Online booking {BookingNumber} has no authorization ID.",
                booking.BookingNumber);

            return Result.Failure(
                "Booking requires payment reconciliation.");
        }

        // TotalAmount currently represents the services subtotal.
        var amountToCapture =
            booking.TotalAmount + booking.ConfirmationFeeAmount;

        // 7. Capture the authorized payment.
        var captureResult = await _paymentGateway.CaptureAsync(
            authorizationId,
            amountToCapture,
            ct);

        if (!captureResult.IsSuccess)
        {
            // A failed request does not prove that Paymob
            // failed to capture the payment.
            _logger.LogWarning(
                "Capture result requires reconciliation for booking {BookingNumber}. Error: {Error}",
                booking.BookingNumber,
                captureResult.ErrorMessage);

            // Keep PendingCapture. Do not blindly retry or void.
            return Result.Failure(
                "Payment confirmation is pending. The booking requires reconciliation.");
        }

        // 8. Capture succeeded.
        // GatewayOperationResult has no TransactionId, so use the
        // existing authorization reference to update the authorized
        // PaymentTransaction in the current domain model.
        try
        {
            booking.ConfirmAfterCapture(authorizationId);

            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            // Capture may have succeeded even if saving confirmation failed.
            _logger.LogError(
                ex,
                "Capture succeeded but booking confirmation could not be saved. Booking: {BookingNumber}",
                booking.BookingNumber);

            return Result.Failure(
                "Payment was processed, but booking confirmation requires reconciliation.");
        }

        _logger.LogInformation(
            "Booking {BookingNumber} accepted and payment captured.",
            booking.BookingNumber);

        return Result.Success(
            "Booking accepted and payment captured successfully.");
    }
}
