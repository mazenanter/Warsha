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
            var cars = await _context.Cars.Include(c => c.CarModel)
                .ThenInclude(cm => cm.CarBrand)
                .ToListAsync();
            return cars;
        }
        public async Task<Car?> GetCarById(int id,CancellationToken ct = default)
        {
            var car = await _context.Cars.Include(c => c.CarModel)
                .ThenInclude(cm => cm.CarBrand)
                .FirstOrDefaultAsync(c => c.Id == id);
            return car;
        }
        public async Task<Car?> GetByModelAndYearAsync(int model, int year, CancellationToken ct = default)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarModelId == model && c.Year == year);
            return car;
        }
        public async Task<Car?> GetByIdWithCorrectionsAsync(int id, CancellationToken ct = default)
    => await _context.Cars
        .Include(c => c.CarModel).ThenInclude(m => m.CarBrand)
        .Include(c => c.Corrections)
        .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
