using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LoyaltyPointRuleRepository
    : Repository<LoyaltyPointRule>, ILoyaltyPointRuleRepository
    {
        public LoyaltyPointRuleRepository(AppDbContext context) : base(context) { }

        public async Task<LoyaltyPointRule?> GetByActionAsync(
            LoyaltyAction action, CancellationToken ct = default)
            => await _context.LoyaltyPointRules
                .FirstOrDefaultAsync(r => r.Action == action && r.IsActive, ct);

        public async Task<IEnumerable<LoyaltyPointRule>> GetAllActiveAsync(
            CancellationToken ct = default)
            => await _context.LoyaltyPointRules
                .Where(r => r.IsActive)
                .ToListAsync(ct);
    }
}
