using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.RedeemReward
{
    public record RedeemRewardCommand(int RewardId) : IRequest<Result<string>>;
}
