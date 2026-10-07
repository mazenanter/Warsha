using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class PaymobService : IPaymentGatewayService
{
    private readonly HttpClient _httpClient;
    private readonly PaymobSettings _settings;
    private readonly ILogger<PaymobService> _logger;

    public PaymobService(
        IHttpClientFactory factory,
        IOptions<PaymobSettings> settings,
        ILogger<PaymobService> logger)
    {
        _httpClient = factory.CreateClient("Paymob");
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<CreatePaymentResult> CreatePaymentAsync(
       CreatePaymentRequest request,
       CancellationToken ct = default)
    {
        try
        {
            var amountCents = ConvertToCents(request.AmountInEgp);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_settings.BaseUrl}/v1/intention/");

            httpRequest.Headers.Add(
                "Authorization",
                $"Token {_settings.SecretKey}");

            httpRequest.Content = JsonContent.Create(new
            {
                amount = amountCents,

                currency = "EGP",

                payment_methods = new[]
                {
                    _settings.IntegrationId
                },

                items = new[]
                {
                    new
                    {
                        name = $"Warsha Booking {request.BookingNumber}",
                        amount = amountCents,
                        description = "Warsha booking payment",
                        quantity = 1
                    }
                },

                billing_data = new
                {
                    apartment = "NA",
                    first_name = request.ClientFirstName,
                    last_name = request.ClientLastName,
                    street = "NA",
                    building = "NA",
                    phone_number = request.ClientPhone,
                    city = "NA",
                    country = "EG",
                    email = request.ClientEmail,
                    floor = "NA",
                    state = "NA",
                    postal_code = "NA",
                    shipping_method = "NA"
                },

                special_reference = request.BookingNumber,

                expiration = 3600,

                notification_url = _settings.NotificationUrl,
            });

            var response = await _httpClient.SendAsync(
                httpRequest,
                ct);

            var responseBody =
                await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Paymob intention failed. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode,
                    responseBody);

                return new CreatePaymentResult(
                    false,
                    null,
                    null,
                    "Paymob intention creation failed");
            }

            var intention =
                JsonSerializer.Deserialize<PaymobIntentionResponse>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (intention is null ||
                string.IsNullOrWhiteSpace(intention.ClientSecret))
            {
                _logger.LogError(
                    "Paymob returned invalid intention response: {Body}",
                    responseBody);

                return new CreatePaymentResult(
                    false,
                    null,
                    null,
                    "Invalid Paymob response");
            }

            return new CreatePaymentResult(
                true,
                intention.ClientSecret,
                intention.IntentionOrderId.ToString(),
                null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Paymob intention creation failed for booking {BookingNumber}",
                request.BookingNumber);

            return new CreatePaymentResult(
                false,
                null,
                null,
                "Payment service unavailable");
        }
    }







    public bool VerifyWebhookSignature(string hmac, Dictionary<string, string> data)
    {
        var fields = new[]
        {
            "amount_cents", "created_at", "currency", "error_occured",
            "has_parent_transaction", "id", "integration_id", "is_3d_secure",
            "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
            "is_voided", "order.id", "owner", "pending",
            "source_data.pan", "source_data.sub_type", "source_data.type", "success"
        };

        var concat = string.Concat(fields.Select(f => data.GetValueOrDefault(f, "")));
        var key = Encoding.UTF8.GetBytes(_settings.HmacSecret);
        var msg = Encoding.UTF8.GetBytes(concat);

        using var sha = new HMACSHA512(key);
        var computed = Convert.ToHexString(sha.ComputeHash(msg)).ToLower();

        return computed == hmac.ToLower();
    }

    public async Task<RefundResult> RefundAsync(
      string transactionId,
      decimal amountInEgp,
      CancellationToken ct = default)
    {
        try
        {
            // هنستخدم authentication المناسب للـ Refund API
            // بعد ما نثبت الـ transaction flow.

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_settings.BaseUrl}/api/acceptance/void_refund/refund");

            request.Content = JsonContent.Create(new
            {
                transaction_id = transactionId,
                amount_cents = ConvertToCents(amountInEgp)
            });

            request.Headers.Add(
                "Authorization",
                $"Token {_settings.SecretKey}");

            var response =
                await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body =
                    await response.Content.ReadAsStringAsync(ct);

                _logger.LogError(
                    "Paymob refund failed. Status: {Status}, Body: {Body}",
                    response.StatusCode,
                    body);

                return new RefundResult(
                    false,
                    "Paymob refund failed");
            }

            return new RefundResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Refund failed for Paymob transaction {TransactionId}",
                transactionId);

            return new RefundResult(false, ex.Message);
        }
    }
    private static int ConvertToCents(decimal amountInEgp)
    {
        return checked((int)Math.Round(
            amountInEgp * 100,
            MidpointRounding.AwayFromZero));
    }
    public class PaymobIntentionResponse
    {
        [JsonPropertyName("intention_order_id")]
        public int IntentionOrderId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("special_reference")]
        public string? SpecialReference { get; set; }
    }
}