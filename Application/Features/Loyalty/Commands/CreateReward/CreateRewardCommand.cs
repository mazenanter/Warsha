using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.CreateReward
{
    public record CreateRewardCommand(
    string Name,
    string Description,
    int PointsCost,
    decimal? DiscountAmount,
    int? ExpiryDays,
    int? RedemptionLimitPerClient
) : IRequest<Result<int>>;
}
