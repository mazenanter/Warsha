namespace Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<CreatePaymentResult> CreatePaymentAsync(
            CreatePaymentRequest request, CancellationToken ct = default);

        bool VerifyWebhookSignature(string hmac, Dictionary<string, string> transactionData);

        Task<RefundResult> RefundAsync(
            string transactionId, decimal amountInEgp, CancellationToken ct = default);
    }
    public record CreatePaymentRequest(
    string BookingNumber,
    decimal AmountInEgp,
    string ClientEmail,
    string ClientPhone,
    string ClientFirstName,
    string ClientLastName);

    public record CreatePaymentResult(
        bool IsSuccess,
        string? ClientSecret,
        string? PaymentOrderId,
        string? ErrorMessage);

    public record RefundResult(bool IsSuccess, string? ErrorMessage);
}
