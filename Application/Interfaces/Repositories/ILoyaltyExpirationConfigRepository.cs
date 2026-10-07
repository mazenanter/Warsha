using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoyaltyExpirationConfigRepository
      : IRepository<LoyaltyExpirationConfig>
    {
        Task<LoyaltyExpirationConfig?> GetCurrentAsync(CancellationToken ct = default);
    }
}
