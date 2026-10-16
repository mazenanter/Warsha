//using Application.Interfaces;
//using Infrastructure.Settings;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Net.Http.Json;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Infrastructure.Services;

//public class PaymobService : IPaymentGatewayService
//{
//    private readonly HttpClient _httpClient;
//    private readonly PaymobSettings _settings;
//    private readonly ILogger<PaymobService> _logger;

//    public PaymobService(
//        IHttpClientFactory factory,
//        IOptions<PaymobSettings> settings,
//        ILogger<PaymobService> logger)
//    {
//        _httpClient = factory.CreateClient("Paymob");
//        _settings = settings.Value;
//        _logger = logger;
//    }

//    public async Task<CreatePaymentResult> CreatePaymentAsync(
//       CreatePaymentRequest request,
//       CancellationToken ct = default)
//    {
//        try
//        {
//            var amountCents = ConvertToCents(request.AmountInEgp);

//            using var httpRequest = new HttpRequestMessage(
//                HttpMethod.Post,
//                $"{_settings.BaseUrl}/v1/intention/");

//            httpRequest.Headers.Add(
//                "Authorization",
//                $"Token {_settings.SecretKey}");

//            httpRequest.Content = JsonContent.Create(new
//            {
//                amount = amountCents,

//                currency = "EGP",

//                payment_methods = new[]
//                {
//                    _settings.IntegrationId
//                },

//                items = new[]
//                {
//                    new
//                    {
//                        name = $"Warsha Booking {request.BookingNumber}",
//                        amount = amountCents,
//                        description = "Warsha booking payment",
//                        quantity = 1
//                    }
//                },

//                billing_data = new
//                {
//                    apartment = "NA",
//                    first_name = request.ClientFirstName,
//                    last_name = request.ClientLastName,
//                    street = "NA",
//                    building = "NA",
//                    phone_number = request.ClientPhone,
//                    city = "NA",
//                    country = "EG",
//                    email = request.ClientEmail,
//                    floor = "NA",
//                    state = "NA",
//                    postal_code = "NA",
//                    shipping_method = "NA"
//                },

//                special_reference = request.BookingNumber,

//                expiration = 3600,

//                notification_url = _settings.NotificationUrl,
//            });

//            var response = await _httpClient.SendAsync(
//                httpRequest,
//                ct);

//            var responseBody =
//                await response.Content.ReadAsStringAsync(ct);

//            if (!response.IsSuccessStatusCode)
//            {
//                _logger.LogError(
//                    "Paymob intention failed. Status: {StatusCode}, Body: {Body}",
//                    response.StatusCode,
//                    responseBody);

//                return new CreatePaymentResult(
//                    false,
//                    null,
//                    null,
//                    "Paymob intention creation failed");
//            }

//            var intention =
//                JsonSerializer.Deserialize<PaymobIntentionResponse>(
//                    responseBody,
//                    new JsonSerializerOptions
//                    {
//                        PropertyNameCaseInsensitive = true
//                    });

//            if (intention is null ||
//                string.IsNullOrWhiteSpace(intention.ClientSecret))
//            {
//                _logger.LogError(
//                    "Paymob returned invalid intention response: {Body}",
//                    responseBody);

//                return new CreatePaymentResult(
//                    false,
//                    null,
//                    null,
//                    "Invalid Paymob response");
//            }

//            return new CreatePaymentResult(
//                true,
//                intention.ClientSecret,
//                intention.IntentionOrderId.ToString(),
//                null);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(
//                ex,
//                "Paymob intention creation failed for booking {BookingNumber}",
//                request.BookingNumber);

//            return new CreatePaymentResult(
//                false,
//                null,
//                null,
//                "Payment service unavailable");
//        }
//    }


//    public Task<GatewayOperationResult> CaptureAsync(
//    string transactionId,
//    decimal amountInEgp,
//    CancellationToken ct = default)
//    {
//        return ExecuteTransactionOperationAsync(
//            "api/acceptance/capture",
//            new
//            {
//                transaction_id = ParseTransactionId(transactionId),
//                amount_cents = ConvertToCents(amountInEgp)
//            },
//            ct);
//    }

//    public Task<GatewayOperationResult> VoidAsync(
//    string transactionId,
//    CancellationToken ct = default)
//    {
//        return ExecuteTransactionOperationAsync(
//            "api/acceptance/void_refund/void",
//            new
//            {
//                transaction_id = ParseTransactionId(transactionId)
//            },
//            ct);
//    }




//    public bool VerifyWebhookSignature(string hmac, Dictionary<string, string> data)
//    {
//        var fields = new[]
//        {
//            "amount_cents", "created_at", "currency", "error_occured",
//            "has_parent_transaction", "id", "integration_id", "is_3d_secure",
//            "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
//            "is_voided", "order.id", "owner", "pending",
//            "source_data.pan", "source_data.sub_type", "source_data.type", "success"
//        };

//        var concat = string.Concat(fields.Select(f => data.GetValueOrDefault(f, "")));
//        var key = Encoding.UTF8.GetBytes(_settings.HmacSecret);
//        var msg = Encoding.UTF8.GetBytes(concat);

//        using var sha = new HMACSHA512(key);
//        var computed = Convert.ToHexString(sha.ComputeHash(msg)).ToLower();

//        return computed == hmac.ToLower();
//    }

//    public async Task<RefundResult> RefundAsync(
//      string transactionId,
//      decimal amountInEgp,
//      CancellationToken ct = default)
//    {
//        try
//        {
//            // هنستخدم authentication المناسب للـ Refund API
//            // بعد ما نثبت الـ transaction flow.

//            var request = new HttpRequestMessage(
//                HttpMethod.Post,
//                $"{_settings.BaseUrl}/api/acceptance/void_refund/refund");

//            request.Content = JsonContent.Create(new
//            {
//                transaction_id = transactionId,
//                amount_cents = ConvertToCents(amountInEgp)
//            });

//            request.Headers.Add(
//                "Authorization",
//                $"Token {_settings.SecretKey}");

//            var response =
//                await _httpClient.SendAsync(request, ct);

//            if (!response.IsSuccessStatusCode)
//            {
//                var body =
//                    await response.Content.ReadAsStringAsync(ct);

//                _logger.LogError(
//                    "Paymob refund failed. Status: {Status}, Body: {Body}",
//                    response.StatusCode,
//                    body);

//                return new RefundResult(
//                    false,
//                    "Paymob refund failed");
//            }

//            return new RefundResult(true, null);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(
//                ex,
//                "Refund failed for Paymob transaction {TransactionId}",
//                transactionId);

//            return new RefundResult(false, ex.Message);
//        }
//    }
//    private static long ParseTransactionId(string transactionId)
//    {
//        if (!long.TryParse(transactionId, out var id) || id <= 0)
//        {
//            throw new ArgumentException(
//                "Invalid Paymob transaction ID.",
//                nameof(transactionId));
//        }

//        return id;
//    }
//    private async Task<GatewayOperationResult>
//    ExecuteTransactionOperationAsync(
//        string endpoint,
//        object payload,
//        CancellationToken ct)
//    {
//        try
//        {
//            using var request = new HttpRequestMessage(
//                HttpMethod.Post,
//                endpoint);

//            request.Headers.Authorization =
//                new System.Net.Http.Headers.AuthenticationHeaderValue(
//                    "Token",
//                    _settings.SecretKey);

//            request.Content = System.Net.Http.Json.JsonContent.Create(payload);

//            using var response = await _httpClient.SendAsync(
//                request,
//                ct);

//            var responseBody = await response.Content.ReadAsStringAsync(ct);

//            if (!response.IsSuccessStatusCode)
//            {
//                _logger.LogError(
//                    "Paymob operation {Endpoint} failed. Status: {StatusCode}. Response: {Response}",
//                    endpoint,
//                    (int)response.StatusCode,
//                    responseBody);

//                return new GatewayOperationResult(
//                    false,
//                    $"Paymob operation failed with status {(int)response.StatusCode}.");
//            }

//            return new GatewayOperationResult(true, null);
//        }
//        catch (OperationCanceledException) when (ct.IsCancellationRequested)
//        {
//            throw;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(
//                ex,
//                "Paymob operation {Endpoint} returned an uncertain result.",
//                endpoint);

//            return new GatewayOperationResult(
//                false,
//                "Payment operation outcome is uncertain. Reconcile with Paymob before retrying.");
//        }
//    }
//    private static int ConvertToCents(decimal amountInEgp)
//    {
//        return checked((int)Math.Round(
//            amountInEgp * 100,
//            MidpointRounding.AwayFromZero));
//    }
//    public class PaymobIntentionResponse
//    {
//        [JsonPropertyName("intention_order_id")]
//        public int IntentionOrderId { get; set; }

//        [JsonPropertyName("id")]
//        public string Id { get; set; } = default!;

//        [JsonPropertyName("client_secret")]
//        public string ClientSecret { get; set; } = default!;

//        [JsonPropertyName("status")]
//        public string Status { get; set; } = default!;

//        [JsonPropertyName("special_reference")]
//        public string? SpecialReference { get; set; }
//    }
//}

using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
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

            var payload = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = new[] { _settings.IntegrationId },

                // Requires an Auth/Capture-capable Paymob integration.
                is_auth = true,

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
                notification_url = _settings.NotificationUrl
            };

            using var httpRequest = CreateRequest(
                HttpMethod.Post,
                "v1/intention/");

            httpRequest.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                ct);

            var responseBody =
                await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Paymob intention failed. Status: {StatusCode}, Body: {Body}",
                    (int)response.StatusCode,
                    responseBody);

                return new CreatePaymentResult(
                    false,
                    null,
                    null,
                    "Paymob intention creation failed.");
            }

            var intention =
                JsonSerializer.Deserialize<PaymobIntentionResponse>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (intention is null ||
                string.IsNullOrWhiteSpace(intention.ClientSecret) ||
                intention.IntentionOrderId <= 0)
            {
                _logger.LogError(
                    "Invalid Paymob intention response: {Body}",
                    responseBody);

                return new CreatePaymentResult(
                    false,
                    null,
                    null,
                    "Invalid Paymob response.");
            }

            return new CreatePaymentResult(
                true,
                intention.ClientSecret,
                intention.IntentionOrderId.ToString(
                    CultureInfo.InvariantCulture),
                null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
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
                "Payment service unavailable.");
        }
    }

    public Task<GatewayOperationResult> CaptureAsync(
        string transactionId,
        decimal amountInEgp,
        CancellationToken ct = default)
    {
        if (amountInEgp <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amountInEgp),
                "Capture amount must be greater than zero.");
        }

        return ExecuteTransactionOperationAsync(
            "api/acceptance/capture",
            new
            {
                transaction_id = ParseTransactionId(transactionId),
                amount_cents = ConvertToCents(amountInEgp)
            },
            ct);
    }

    public Task<GatewayOperationResult> VoidAsync(
        string transactionId,
        CancellationToken ct = default)
    {
        return ExecuteTransactionOperationAsync(
            "api/acceptance/void_refund/void",
            new
            {
                transaction_id = ParseTransactionId(transactionId)
            },
            ct);
    }

    public async Task<RefundResult> RefundAsync(
        string transactionId,
        decimal amountInEgp,
        CancellationToken ct = default)
    {
        if (amountInEgp <= 0)
        {
            return new RefundResult(
                false,
                "Refund amount must be greater than zero.");
        }

        var result = await ExecuteTransactionOperationAsync(
            "api/acceptance/void_refund/refund",
            new
            {
                transaction_id = ParseTransactionId(transactionId),
                amount_cents = ConvertToCents(amountInEgp)
            },
            ct);

        return new RefundResult(
            result.IsSuccess,
            result.ErrorMessage);
    }

    public bool VerifyWebhookSignature(
        string hmac,
        Dictionary<string, string> data)
    {
        if (string.IsNullOrWhiteSpace(hmac) ||
            string.IsNullOrWhiteSpace(_settings.HmacSecret))
        {
            return false;
        }

        var fields = new[]
        {
            "amount_cents",
            "created_at",
            "currency",
            "error_occured",
            "has_parent_transaction",
            "id",
            "integration_id",
            "is_3d_secure",
            "is_auth",
            "is_capture",
            "is_refunded",
            "is_standalone_payment",
            "is_voided",
            "order.id",
            "owner",
            "pending",
            "source_data.pan",
            "source_data.sub_type",
            "source_data.type",
            "success"
        };

        var concatenatedValues = string.Concat(
            fields.Select(field => data.GetValueOrDefault(field, "")));

        var key = Encoding.UTF8.GetBytes(_settings.HmacSecret);
        var message = Encoding.UTF8.GetBytes(concatenatedValues);

        using var hmacSha512 = new HMACSHA512(key);

        var computedHash = hmacSha512.ComputeHash(message);

        byte[] suppliedHash;

        try
        {
            suppliedHash = Convert.FromHexString(hmac);
        }
        catch (FormatException)
        {
            return false;
        }

        return suppliedHash.Length == computedHash.Length &&
               CryptographicOperations.FixedTimeEquals(
                   suppliedHash,
                   computedHash);
    }

    private async Task<GatewayOperationResult>
        ExecuteTransactionOperationAsync(
            string endpoint,
            object payload,
            CancellationToken ct)
    {
        try
        {
            using var request = CreateRequest(
                HttpMethod.Post,
                endpoint);

            request.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(
                request,
                ct);

            var responseBody =
                await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Paymob operation {Endpoint} failed. Status: {StatusCode}, Body: {Body}",
                    endpoint,
                    (int)response.StatusCode,
                    responseBody);

                return new GatewayOperationResult(
                    false,
                    $"Paymob operation failed with status {(int)response.StatusCode}.");
            }

            return new GatewayOperationResult(true, null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Paymob operation {Endpoint} returned an uncertain result.",
                endpoint);

            return new GatewayOperationResult(
                false,
                "Payment operation outcome is uncertain. Reconcile with Paymob before retrying.");
        }
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string relativeEndpoint)
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/') + "/";
        var endpoint = relativeEndpoint.TrimStart('/');

        var request = new HttpRequestMessage(
            method,
            new Uri(new Uri(baseUrl), endpoint));

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Token",
                _settings.SecretKey);

        return request;
    }

    private static long ParseTransactionId(string transactionId)
    {
        if (!long.TryParse(
                transactionId,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var id) ||
            id <= 0)
        {
            throw new ArgumentException(
                "Invalid Paymob transaction ID.",
                nameof(transactionId));
        }

        return id;
    }

    private static int ConvertToCents(decimal amountInEgp)
    {
        if (amountInEgp <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amountInEgp),
                "Amount must be greater than zero.");
        }

        return checked((int)Math.Round(
            amountInEgp * 100m,
            MidpointRounding.AwayFromZero));
    }

    public sealed class PaymobIntentionResponse
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
