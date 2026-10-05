using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetAvailableRewards
{
    public class GetAvailableRewardsQueryHandler
     : IRequestHandler<GetAvailableRewardsQuery, Result<IEnumerable<RewardDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetAvailableRewardsQueryHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<RewardDto>>> Handle(
            GetAvailableRewardsQuery request, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;
            var wallet = await _unitOfWork.LoyaltyWallets.GetByClientIdAsync(clientId, ct);
            var rewards = await _unitOfWork.LoyaltyRewards.GetActiveAsync(ct);

            var result = rewards.Select(r =>
            {
                var canRedeem = (wallet?.AvailablePoints ?? 0) >= r.PointsCost;
                var pointsRemaining = canRedeem ? null as int?
                    : r.PointsCost - (wallet?.AvailablePoints ?? 0);

                return new RewardDto(
                    r.Id, r.Name, r.Description,
                    r.PointsCost, r.DiscountAmount,
                    canRedeem, pointsRemaining);
            })
            .OrderBy(r => r.PointsCost);

            return Result<IEnumerable<RewardDto>>.Success(result, "Available rewards retrieved successfully");
        }
    }
}
