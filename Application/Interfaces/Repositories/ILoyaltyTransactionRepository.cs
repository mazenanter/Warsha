using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
    public interface ILoyaltyTransactionRepository : IRepository<LoyaltyTransaction>
    {
        Task<IEnumerable<LoyaltyTransaction>> GetByClientIdAsync(
            int clientId, int page, int pageSize, CancellationToken ct = default);

        Task<bool> HasEarnedForActionAsync(
            int clientId, LoyaltyAction action, CancellationToken ct = default);

        Task<LoyaltyTransaction?> GetLastByActionAsync(
            int clientId, LoyaltyAction action, CancellationToken ct = default);

        Task<IEnumerable<LoyaltyTransaction>> GetPendingByBookingAsync(
            int bookingId, CancellationToken ct = default);

        Task<IEnumerable<(int ClientId, int PendingPoints, int AvailablePoints)>>
            GetClientsForExpirationAsync(int inactivityMonths, CancellationToken ct = default);
    }
}
