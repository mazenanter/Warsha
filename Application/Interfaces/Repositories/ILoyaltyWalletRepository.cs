using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoyaltyWalletRepository : IRepository<LoyaltyWallet>
    {
        Task<LoyaltyWallet?> GetByClientIdAsync(int clientId, CancellationToken ct = default);
    }
}
