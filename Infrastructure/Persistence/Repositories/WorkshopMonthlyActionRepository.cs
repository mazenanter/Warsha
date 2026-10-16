using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WorkshopMonthlyActionRepository
        : Repository<WorkshopMonthlyAction>,
          IWorkshopMonthlyActionRepository
    {
        private readonly AppDbContext _context;

        public WorkshopMonthlyActionRepository(
            AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<bool> ExistsAsync(
            int workshopId,
            int bookingId,
            WorkshopActionType actionType,
            CancellationToken cancellationToken = default)
        {
            return _context.Set<WorkshopMonthlyAction>()
                .AnyAsync(
                    x => x.WorkshopId == workshopId
                      && x.BookingId == bookingId
                      && x.ActionType == actionType,
                    cancellationToken);
        }
    }
}
