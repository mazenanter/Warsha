using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(AppDbContext context) : base(context) { }

        public async Task<Trip?> GetByIdWithCarAsync(int id, CancellationToken ct = default)
            => await _context.Trips
                .Include(t => t.Car)
                .FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<Trip?> GetActiveByCarIdAsync(int carId, CancellationToken ct = default)
            => await _context.Trips
                .Include(t => t.Car)
                .FirstOrDefaultAsync(t =>
                    t.CarId == carId &&
                    t.Status == TripStatus.Active, ct);

        public async Task<IEnumerable<Trip>> GetByCarIdAsync(
            int carId, int page, int pageSize, CancellationToken ct = default)
            => await _context.Trips
                .Where(t => t.CarId == carId)
                .OrderByDescending(t => t.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
    }
}
