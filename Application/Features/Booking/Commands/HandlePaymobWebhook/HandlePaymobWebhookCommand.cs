using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.HandlePaymobWebhook
{
    public record HandlePaymobWebhookCommand(
    Dictionary<string, string> TransactionData,
    string Hmac
) : IRequest<Result>;
}
