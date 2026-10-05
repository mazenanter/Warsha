using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoyaltyRewardRepository : IRepository<LoyaltyReward>
    {
        Task<IEnumerable<LoyaltyReward>> GetActiveAsync(CancellationToken ct = default);

        Task<int> GetClientRedemptionCountAsync(
            int clientId, int rewardId, CancellationToken ct = default);
    }
}
