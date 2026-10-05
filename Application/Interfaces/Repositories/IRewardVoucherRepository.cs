using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IRewardVoucherRepository : IRepository<RewardVoucher>
    {
        Task<RewardVoucher?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<RewardVoucher>> GetByClientIdAsync(
            int clientId, CancellationToken ct = default);
    }
}
