using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PlatformFeeConfigRepository
     : Repository<PlatformFeeConfig>, IPlatformFeeConfigRepository
    {
        public PlatformFeeConfigRepository(AppDbContext context) : base(context) { }

        public async Task<PlatformFeeConfig?> GetCurrentAsync(CancellationToken ct = default)
            => await _context.PlatformFeeConfigs
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);
    }
}
