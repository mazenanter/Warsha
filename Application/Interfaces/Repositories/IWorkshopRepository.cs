using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IWorkshopRepository : IRepository<Workshop>
    {
        Task<Workshop?> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<Workshop?> GetByUserIdWithOffersAsync(
    int userId,
    CancellationToken ct = default);
        Task<Workshop?> GetByUserIdWithServicesAsync(
    int userId,
    CancellationToken ct = default);
        Task<Workshop?> GetByUserIdWithCarBrandsAsync(
    int userId,
    CancellationToken ct = default);
        Task<Workshop?> GetByIdWithCarBrandsAsync(
    int workshopId,
    CancellationToken ct = default);
        Task<Workshop?> GetByIdWithServicesAsync(int id, CancellationToken ct = default);

        Task<Workshop?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);

        Task<Workshop?> GetByUserId(int userId, CancellationToken ct = default);

    }
}
