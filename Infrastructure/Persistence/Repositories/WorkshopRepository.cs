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

        public async Task<Workshop?> GetByIdWithServicesAsync(int id, CancellationToken ct = default)
            => await _context.Workshops
                .Include(w => w.Services)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<Workshop?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
            => await _context.Workshops
                .Include(w => w.Services)
                .FirstOrDefaultAsync(w => w.Id == id, ct);
    }
}
