using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WorkshopRepository : Repository<Workshop>, IWorkshopRepository
    {

        public WorkshopRepository(AppDbContext context) : base(context) { }
        public async Task<Workshop?> GetByUserIdAsync(int userId, CancellationToken ct = default)
          => await _context.Workshops
              .FirstOrDefaultAsync(w => w.UserId == userId, ct);
        public async Task<Workshop?> GetByUserIdWithOffersAsync(
    int userId,
    CancellationToken ct = default)
        {
            return await _context.Workshops
                .Include(w => w.Offers)
                .ThenInclude(o => o.WorkshopService)
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);
        }
        public async Task<Workshop?> GetByUserIdWithServicesAsync(
    int userId,
    CancellationToken ct = default)
        {
            return await _context.Workshops
                .Include(w => w.Services)
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);
        }
        public async Task<Workshop?> GetByIdWithServicesAsync(int id, CancellationToken ct = default)
            => await _context.Workshops
                .Include(w => w.Services)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<Workshop?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
            => await _context.Workshops
                .Include(w => w.Services)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<Workshop?> GetByUserId(int userId, CancellationToken ct = default)
       => await _context.Workshops.FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<Workshop?> GetByUserIdWithCarBrandsAsync(
    int userId,
    CancellationToken ct = default)
        {
            return await _context.Workshops
                .Include(w => w.SupportedCarBrands)
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);

        }

            public async Task<Workshop?> GetByIdWithCarBrandsAsync(
    int workshopId,
    CancellationToken ct = default)
        {
            return await _context.Workshops
                .Include(w => w.SupportedCarBrands)
                .FirstOrDefaultAsync(w => w.Id == workshopId, ct);
        }
    }
    }

