
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.Commands.HandlePaymobWebhook;

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

    public async Task<Result> Handle(
        HandlePaymobWebhookCommand command,
        CancellationToken ct)
    {
        var data = command.TransactionData;

        if (!_paymentGateway.VerifyWebhookSignature(
                command.Hmac,
                data))
        {
            _logger.LogWarning("Invalid Paymob HMAC signature.");
            return Result.Failure("Invalid signature.");
        }

        var orderId = GetValue(data, "order.id");
        var transactionId = GetValue(data, "id");
        var success = IsTrue(data, "success");
        var pending = IsTrue(data, "pending");
        var isAuth = IsTrue(data, "is_auth");
        var isCapture = IsTrue(data, "is_capture");
        var isVoided = IsTrue(data, "is_voided");
        var isRefunded = IsTrue(data, "is_refunded");

        if (string.IsNullOrWhiteSpace(orderId))
        {
            _logger.LogWarning(
                "Paymob webhook did not contain an order ID.");

            return Result.Failure("Missing payment order ID.");
        }

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            _logger.LogWarning(
                "Paymob webhook did not contain a transaction ID for order {OrderId}.",
                orderId);

            return Result.Failure("Missing transaction ID.");
        }

        var booking = await _unitOfWork.Bookings
            .GetByPaymentOrderIdAsync(orderId, ct);

        if (booking is null)
        {
            _logger.LogWarning(
                "No booking found for Paymob order {OrderId}.",
                orderId);

            return Result.Failure("Booking not found.");
        }

        // Pending notifications are not final payment results.
        if (pending)
        {
            _logger.LogInformation(
                "Paymob transaction {TransactionId} is still pending.",
                transactionId);

            return Result.Success("Payment is still processing.");
        }

        // These events do not confirm a booking.
        // Their state changes must be handled by the corresponding
        // cancellation/refund workflow and reconciled with Paymob.
        if (isVoided || isRefunded)
        {
            _logger.LogInformation(
                "Received Paymob void/refund notification for booking {BookingNumber}.",
                booking.BookingNumber);

            return Result.Success(
                "Void/refund notification received.");
        }

        if (!success)
        {
            // Only a final authorization failure can fail a booking
            // that is still waiting for authorization.
            if (isAuth &&
                booking.BookingStatus == BookingStatus.PendingPaymentAuth)
            {
                var reason = GetValue(data, "data.message");

                if (string.IsNullOrWhiteSpace(reason))
                    reason = "Payment authorization failed.";

                booking.FailOnlinePayment(reason);

                await _unitOfWork.SaveChangesAsync(ct);

                _logger.LogWarning(
                    "Authorization failed for booking {BookingNumber}. Reason: {Reason}",
                    booking.BookingNumber,
                    reason);

                return Result.Success(
                    "Authorization failure processed.");
            }

            _logger.LogInformation(
                "Ignoring unsuccessful or non-final Paymob notification for booking {BookingNumber}. Auth={IsAuth}, Capture={IsCapture}",
                booking.BookingNumber,
                isAuth,
                isCapture);

            return Result.Success(
                "Non-final payment notification received.");
        }

        if (isAuth)
        {
            if (booking.BookingStatus != BookingStatus.PendingPaymentAuth)
            {
                _logger.LogInformation(
                    "Ignoring duplicate or outdated authorization notification for booking {BookingNumber}. Current status: {Status}",
                    booking.BookingNumber,
                    booking.BookingStatus);

                return Result.Success(
                    "Authorization notification already processed or no longer applicable.");
            }

            booking.MarkAuthorizationSucceeded(transactionId);

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Authorization succeeded for booking {BookingNumber}. Waiting for workshop approval.",
                booking.BookingNumber);

            return Result.Success(
                "Authorization succeeded. Booking is awaiting workshop approval.");
        }

        if (isCapture)
        {
            if (booking.BookingStatus != BookingStatus.PendingCapture)
            {
                _logger.LogInformation(
                    "Ignoring duplicate or outdated capture notification for booking {BookingNumber}. Current status: {Status}",
                    booking.BookingNumber,
                    booking.BookingStatus);

                return Result.Success(
                    "Capture notification already processed or no longer applicable.");
            }

            booking.ConfirmAfterCapture(transactionId);

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Capture succeeded and booking {BookingNumber} was confirmed.",
                booking.BookingNumber);

            return Result.Success(
                "Payment captured and booking confirmed.");
        }

        _logger.LogInformation(
            "Paymob transaction {TransactionId} does not represent an authorization or capture event. Booking: {BookingNumber}.",
            transactionId,
            booking.BookingNumber);

        return Result.Success(
            "Payment notification received; no booking transition was required.");
    }

    private static string GetValue(
        Dictionary<string, string> data,
        string key)
    {
        return data.GetValueOrDefault(key, string.Empty)?.Trim()
            ?? string.Empty;
    }

    private static bool IsTrue(
        Dictionary<string, string> data,
        string key)
    {
        return bool.TryParse(GetValue(data, key), out var value)
            && value;
    }
}



//using Application.Interfaces;
//using Domain.Common;
//using Domain.Enums;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace Application.Features.Booking.Commands.HandlePaymobWebhook
//{
//    public class HandlePaymobWebhookCommandHandler
//    : IRequestHandler<HandlePaymobWebhookCommand, Result>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IPaymentGatewayService _paymentGateway;
//        private readonly ILogger<HandlePaymobWebhookCommandHandler> _logger;

//        public HandlePaymobWebhookCommandHandler(
//            IUnitOfWork unitOfWork,
//            IPaymentGatewayService paymentGateway,
//            ILogger<HandlePaymobWebhookCommandHandler> logger)
//        {
//            _unitOfWork = unitOfWork;
//            _paymentGateway = paymentGateway;
//            _logger = logger;
//        }

//        public async Task<Result> Handle(HandlePaymobWebhookCommand command, CancellationToken ct)
//        {
//            if (!_paymentGateway.VerifyWebhookSignature(command.Hmac, command.TransactionData))
//            {
//                _logger.LogWarning("Invalid Paymob HMAC signature");
//                return Result.Failure("Invalid signature");
//            }

//            var orderId = command.TransactionData.GetValueOrDefault("order.id", "");
//            var success = command.TransactionData.GetValueOrDefault("success", "") == "true";
//            var txId = command.TransactionData.GetValueOrDefault("id", "");
//            var pending = command.TransactionData.GetValueOrDefault("pending", "") == "true";

//            if (pending) return Result.Failure("Payment is still processing");

//            var booking = await _unitOfWork.Bookings.GetByPaymentOrderIdAsync(orderId, ct);
//            if (booking is null)
//            {
//                _logger.LogWarning("No booking for Paymob order {OrderId}", orderId);
//                return Result.Failure("Booking not found");
//            }

//            if (booking.BookingStatus != BookingStatus.Pending)
//                return Result.Failure("Booking is already confirmed or failed");

//            if (success)
//            {
//                booking.ConfirmOnlinePayment(txId);
//                _logger.LogInformation("Booking {Number} confirmed", booking.BookingNumber);
//            }
//            else
//            {
//                var reason = command.TransactionData.GetValueOrDefault("data.message", "Payment failed");
//                booking.FailOnlinePayment(reason);
//                _logger.LogWarning("Booking {Number} payment failed: {Reason}", booking.BookingNumber, reason);
//            }

//            await _unitOfWork.SaveChangesAsync(ct);
//            return Result.Success("Payment processed successfully");
//        }
//    }
//}
