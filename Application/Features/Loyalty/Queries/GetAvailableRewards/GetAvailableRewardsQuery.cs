using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetAvailableRewards
{
    public record GetAvailableRewardsQuery : IRequest<Result<IEnumerable<RewardDto>>>;

    public record RewardDto(
        int Id,
        string Name,
        string Description,
        int PointsCost,
        decimal? DiscountAmount,
        bool CanRedeem,
        int? PointsRemaining
    );
}
