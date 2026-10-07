using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LoyaltyExpirationConfigRepository
    : Repository<LoyaltyExpirationConfig>, ILoyaltyExpirationConfigRepository
    {
        public LoyaltyExpirationConfigRepository(AppDbContext context) : base(context) { }

        public async Task<LoyaltyExpirationConfig?> GetCurrentAsync(
            CancellationToken ct = default)
            => await _context.LoyaltyExpirationConfigs
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(ct);
    }
}
