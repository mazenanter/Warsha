using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Commands.CancelBooking
{
    public class CancelBookingByClientCommandHandler
    : IRequestHandler<CancelBookingByClientCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGateway;

        public CancelBookingByClientCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result> Handle(CancelBookingByClientCommand command, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result.Failure("Booking not found");
            if (booking.ClientId != clientId) return Result.Failure("Unauthorized");

            booking.CancelByClient(command.Reason);

            if (booking.PaymentType == PaymentType.Online)
            {
                var successTx = booking.PaymentTransactions
                    .FirstOrDefault(t => t.Status == PaymentTransactionStatus.Successful);

                if (successTx?.ProviderReference != null)
                {
                    await _paymentGateway.RefundAsync(
                        successTx.ProviderReference, booking.ConfirmationFeeAmount, ct);
                    successTx.MarkRefunded();
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(
                 "Booking cancelled. Confirmation fee will be refunded within 3-5 business days."
              );
        }
    }
}
