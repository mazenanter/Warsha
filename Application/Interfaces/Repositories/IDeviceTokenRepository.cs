using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IDeviceTokenRepository : IRepository<DeviceToken>
    {
        Task<DeviceToken?> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<IEnumerable<string>> GetTokensByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
