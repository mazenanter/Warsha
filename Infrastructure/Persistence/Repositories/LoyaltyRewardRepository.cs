using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LoyaltyRewardRepository
     : Repository<LoyaltyReward>, ILoyaltyRewardRepository
    {
        public LoyaltyRewardRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<LoyaltyReward>> GetActiveAsync(
            CancellationToken ct = default)
            => await _context.LoyaltyRewards
                .Where(r =>
                    r.IsActive &&
                    (r.StartAt == null || r.StartAt <= DateTime.UtcNow) &&
                    (r.EndAt == null || r.EndAt >= DateTime.UtcNow))
                .OrderBy(r => r.PointsCost)
                .ToListAsync(ct);

        public async Task<int> GetClientRedemptionCountAsync(
            int clientId, int rewardId, CancellationToken ct = default)
            => await _context.LoyaltyTransactions
                .CountAsync(t =>
                    t.ClientId == clientId &&
                    t.RewardId == rewardId &&
                    t.Type == Domain.Enums.LoyaltyTransactionType.Redeem, ct);
    }
}
