using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetByReferralCodeAsync(
            string referralCode,
            CancellationToken ct = default);
    }
}
