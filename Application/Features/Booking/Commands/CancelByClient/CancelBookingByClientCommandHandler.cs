using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.Commands.CancelBooking
{
    public class CancelBookingByClientCommandHandler
        : IRequestHandler<CancelBookingByClientCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly ILogger<CancelBookingByClientCommandHandler> _logger;

        public CancelBookingByClientCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGateway,
            ILogger<CancelBookingByClientCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task<Result> Handle(
            CancelBookingByClientCommand command,
            CancellationToken ct)
        {
            if (_currentUser.ClientId is not int clientId)
                return Result.Failure("Current user is not a client.");

            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null)
                return Result.Failure("Booking not found.");

            if (booking.ClientId != clientId)
                return Result.Failure("Unauthorized.");

            var paymentTransaction = booking.PaymentTransactions
                .FirstOrDefault(t =>
                    t.Status == PaymentTransactionStatus.Authorized ||
                    t.Status == PaymentTransactionStatus.Captured);

            // Process the provider operation before mutating the booking.
            if (booking.PaymentType == PaymentType.Online &&
                paymentTransaction is not null)
            {
                if (paymentTransaction.Status ==
                    PaymentTransactionStatus.Authorized)
                {
                    if (string.IsNullOrWhiteSpace(
                        paymentTransaction.AuthorizationId))
                    {
                        return Result.Failure(
                            "Payment authorization reference is missing.");
                    }

                    var voidResult = await _paymentGateway.VoidAsync(
                        paymentTransaction.AuthorizationId,
                        ct);

                    if (!voidResult.IsSuccess)
                    {
                        _logger.LogWarning(
                            "Void failed for booking {BookingId}: {Error}",
                            booking.Id,
                            voidResult.ErrorMessage);

                        return Result.Failure(
                            "Could not confirm payment authorization cancellation. " +
                            "Please check the payment status before retrying.");
                    }
                }
                else if (paymentTransaction.Status ==
                         PaymentTransactionStatus.Captured)
                {
                    if (string.IsNullOrWhiteSpace(
                        paymentTransaction.ProviderReference))
                    {
                        return Result.Failure(
                            "Payment reference is missing.");
                    }

                    var refundResult = await _paymentGateway.RefundAsync(
                        paymentTransaction.ProviderReference,
                        paymentTransaction.Amount,
                        ct);

                    if (!refundResult.IsSuccess)
                    {
                        _logger.LogWarning(
                            "Refund failed for booking {BookingId}: {Error}",
                            booking.Id,
                            refundResult.ErrorMessage);

                        return Result.Failure(
                            "The refund could not be confirmed. " +
                            "The booking was not cancelled; verify the payment " +
                            "status before retrying.");
                    }
                }
            }

            await using var dbTransaction =
                await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                // Revalidate the booking state inside the persistence transaction
                // in a production implementation using appropriate concurrency control.
                booking.CancelByClient(command.Reason);

                if (paymentTransaction is not null)
                {
                    if (paymentTransaction.Status ==
                        PaymentTransactionStatus.Authorized)
                    {
                        paymentTransaction.MarkVoided();
                    }
                    else if (paymentTransaction.Status ==
                             PaymentTransactionStatus.Captured)
                    {
                        paymentTransaction.MarkRefunded();

                        var confirmationFee = booking.Fees.FirstOrDefault(
                            f => f.FeeType ==
                                 BookingFeeType.ClientConfirmation);

                        confirmationFee?.MarkRefunded();
                    }
                }

                await _unitOfWork.SaveChangesAsync(ct);
                await dbTransaction.CommitAsync(ct);

                return Result.Success(
                    "Booking cancelled successfully. Payment processing completed.");
            }
            catch (DomainException ex)
            {
                await dbTransaction.RollbackAsync(ct);
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync(ct);

                _logger.LogError(
                    ex,
                    "Failed to persist cancellation for booking {BookingId}",
                    booking.Id);

                return Result.Failure(
                    "The payment operation may have completed, but saving the " +
                    "cancellation failed. Check payment status before retrying.");
            }
        }
    }
}
