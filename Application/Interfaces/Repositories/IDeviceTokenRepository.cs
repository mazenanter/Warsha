using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IDeviceTokenRepository : IRepository<DeviceToken>
    {
         Task<DeviceToken?> GetByUserAndDeviceAsync(
     int userId,
     string deviceId,
     CancellationToken ct = default);
        Task<IEnumerable<string>> GetTokensByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
