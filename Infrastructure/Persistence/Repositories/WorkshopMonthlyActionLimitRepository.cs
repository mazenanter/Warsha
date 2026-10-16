using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WorkshopMonthlyActionLimitRepository
       : Repository<WorkshopMonthlyActionLimit>,
         IWorkshopMonthlyActionLimitRepository
    {
        private readonly AppDbContext _context;

        public WorkshopMonthlyActionLimitRepository(
            AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<WorkshopMonthlyActionLimit?> GetByWorkshopAndPeriodAsync(
            int workshopId,
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            return _context.Set<WorkshopMonthlyActionLimit>()
                .FirstOrDefaultAsync(
                    x => x.WorkshopId == workshopId
                      && x.Year == year
                      && x.Month == month,
                    cancellationToken);
        }
    }
}
