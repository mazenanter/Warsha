using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<Trip?> GetByIdWithCarAsync(int id, CancellationToken ct = default);
        Task<Trip?> GetActiveByCarIdAsync(int carId, CancellationToken ct = default);
        Task<IEnumerable<Trip>> GetByCarIdAsync(
            int carId, int page, int pageSize, CancellationToken ct = default);
    }
}
