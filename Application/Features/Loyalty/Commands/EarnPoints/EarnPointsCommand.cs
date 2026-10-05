using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Loyalty.Commands.EarnPoints
{
    public record EarnPointsCommand(
    int ClientId,
    LoyaltyAction Action,
    string Reason,
    int? BookingId = null
) : IRequest<Result>;
}
