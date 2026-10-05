using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetMyWallet
{
    public record GetMyWalletQuery : IRequest<Result<WalletDto>>;

    public record WalletDto(
        int AvailablePoints,
        int PendingPoints,
        int LifetimeEarned,
        int LifetimeRedeemed,
        LoyaltyRewardProgressDto? NextReward
    );

    public record LoyaltyRewardProgressDto(
        string RewardName,
        int PointsCost,
        int PointsRemaining,
        double ProgressPercent
    );
}
