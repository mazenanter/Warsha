using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IWorkshopMonthlyActionLimitRepository
    : IRepository<WorkshopMonthlyActionLimit>
    {
        Task<WorkshopMonthlyActionLimit?> GetByWorkshopAndPeriodAsync(
            int workshopId,
            int year,
            int month,
            CancellationToken ct = default);
    }
}
