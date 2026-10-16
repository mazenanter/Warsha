using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        public CarRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Car>> GetAllCars(CancellationToken ct = default)
        {
            return await _context.Cars
                .Where(c => !c.IsDeleted)
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.CarBrand)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<Car>> GetByClientIdAsync(
    int clientId,
    CancellationToken ct = default)
        {
            return await _context.Cars
                .Where(c => c.ClientId == clientId && !c.IsDeleted)
                .Include(c => c.CarModel)
                    .ThenInclude(m => m.CarBrand)
                .ToListAsync(ct);
        }
        public async Task<Car?> GetCarById(int id,CancellationToken ct = default)
        {
            return await _context.Cars
                .Where(c => !c.IsDeleted)
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.CarBrand)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }
        public async Task<Car?> GetByModelAndYearAsync(int model, int year, CancellationToken ct = default)
        {
            return await _context.Cars
               .FirstOrDefaultAsync(
                   c => !c.IsDeleted
                        && c.CarModelId == model
                        && c.Year == year,
                   ct);
        }
        public async Task<Car?> GetByIdWithCorrectionsAsync(int id, CancellationToken ct = default)
    => await _context.Cars
                .Where(c => !c.IsDeleted)
                .Include(c => c.CarModel)
                    .ThenInclude(m => m.CarBrand)
                .Include(c => c.Corrections)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<Car?> GetByIdForClientAsync(
    int carId,
    int userId,
    CancellationToken ct = default)
        {
            return await _context.Cars
                .Where(c =>
                    c.Id == carId &&
                    !c.IsDeleted &&
                    c.Client.UserId == userId)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(ct);
        }
    }
}
