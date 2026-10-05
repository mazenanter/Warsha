using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPlatformFeeConfigRepository : IRepository<PlatformFeeConfig>
    {
        public Task<PlatformFeeConfig?> GetCurrentAsync(CancellationToken ct = default);
    }
}
