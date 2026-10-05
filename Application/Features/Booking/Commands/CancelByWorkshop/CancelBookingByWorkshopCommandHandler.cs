using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Commands.CancelByWorkshop
{
    public class CancelBookingByWorkshopCommandHandler
    : IRequestHandler<CancelBookingByWorkshopCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGateway;

        public CancelBookingByWorkshopCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result> Handle(CancelBookingByWorkshopCommand command, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result.Failure("Booking not found");
            if (booking.WorkshopId != workshopId) return Result.Failure("Unauthorized");

            booking.CancelByWorkshop(command.Reason);

            if (booking.PaymentType == PaymentType.Online)
            {
                var successTx = booking.PaymentTransactions
                    .FirstOrDefault(t => t.Status == PaymentTransactionStatus.Successful);

                if (successTx?.ProviderReference != null)
                {
                    var refundResult = await _paymentGateway.RefundAsync(
              successTx.ProviderReference,
              successTx.Amount,  
              ct);

                    if (refundResult.IsSuccess)
                        successTx.MarkRefunded();
                    
                      
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success("Booking cancelled. Client refund processed and penalty recorded.");
        }
    }
}
