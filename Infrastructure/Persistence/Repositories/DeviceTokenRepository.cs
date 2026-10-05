using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DeviceTokenRepository : Repository<DeviceToken>, IDeviceTokenRepository
    {
        private readonly AppDbContext _context;

        public DeviceTokenRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<DeviceToken?> GetByUserIdAsync(
        int userId, CancellationToken ct = default)
        => await _context.DeviceTokens
            .FirstOrDefaultAsync(dt => dt.UserId == userId, ct);

        public async Task<IEnumerable<string>> GetTokensByUserIdAsync(
            int userId, CancellationToken ct = default)
            => await _context.DeviceTokens
                .Where(dt => dt.UserId == userId)
                .Select(dt => dt.Token)
                .ToListAsync(ct);
    }
}
