using Application.Features.Booking.Commands.HandlePaymobWebhook;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentWebhookController : ApiControllerBase
    {
        [HttpPost("paymob/webhook")]
        public async Task<IActionResult> PaymobWebhook(
        [FromQuery] string hmac,
        [FromBody] Dictionary<string, string> transactionData,
        CancellationToken ct)
        {
            var result = await Mediator.Send(
                new HandlePaymobWebhookCommand(transactionData, hmac), ct);

       
            return HandleResult(result);
        }
    }
}
