using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LoyaltyWalletRepository
      : Repository<LoyaltyWallet>, ILoyaltyWalletRepository
    {
        public LoyaltyWalletRepository(AppDbContext context) : base(context) { }

        public async Task<LoyaltyWallet?> GetByClientIdAsync(
            int clientId, CancellationToken ct = default)
            => await _context.LoyaltyWallets
                .FirstOrDefaultAsync(w => w.ClientId == clientId, ct);
    }
}
