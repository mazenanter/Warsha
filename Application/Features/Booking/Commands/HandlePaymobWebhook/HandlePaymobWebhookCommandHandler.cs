using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.Commands.HandlePaymobWebhook
{
    public class HandlePaymobWebhookCommandHandler
    : IRequestHandler<HandlePaymobWebhookCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly ILogger<HandlePaymobWebhookCommandHandler> _logger;

        public HandlePaymobWebhookCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentGatewayService paymentGateway,
            ILogger<HandlePaymobWebhookCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task<Result> Handle(HandlePaymobWebhookCommand command, CancellationToken ct)
        {
            if (!_paymentGateway.VerifyWebhookSignature(command.Hmac, command.TransactionData))
            {
                _logger.LogWarning("Invalid Paymob HMAC signature");
                return Result.Failure("Invalid signature");
            }

            var orderId = command.TransactionData.GetValueOrDefault("order.id", "");
            var success = command.TransactionData.GetValueOrDefault("success", "") == "true";
            var txId = command.TransactionData.GetValueOrDefault("id", "");
            var pending = command.TransactionData.GetValueOrDefault("pending", "") == "true";

            if (pending) return Result.Failure("Payment is still processing");

            var booking = await _unitOfWork.Bookings.GetByPaymentOrderIdAsync(orderId, ct);
            if (booking is null)
            {
                _logger.LogWarning("No booking for Paymob order {OrderId}", orderId);
                return Result.Failure("Booking not found");
            }

            if (booking.BookingStatus != BookingStatus.Pending)
                return Result.Failure("Booking is already confirmed or failed");

            if (success)
            {
                booking.ConfirmOnlinePayment(txId);
                _logger.LogInformation("Booking {Number} confirmed", booking.BookingNumber);
            }
            else
            {
                var reason = command.TransactionData.GetValueOrDefault("data.message", "Payment failed");
                booking.FailOnlinePayment(reason);
                _logger.LogWarning("Booking {Number} payment failed: {Reason}", booking.BookingNumber, reason);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success("Payment processed successfully");
        }
    }
}
