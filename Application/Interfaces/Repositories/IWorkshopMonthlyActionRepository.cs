using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IWorkshopMonthlyActionRepository
    : IRepository<WorkshopMonthlyAction>
    {
        Task<bool> ExistsAsync(
            int workshopId,
            int bookingId,
            WorkshopActionType actionType,
            CancellationToken ct = default);
    }
}
