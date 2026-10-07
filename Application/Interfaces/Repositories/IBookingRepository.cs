using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<int> CountCompletedByClientAsync(
            int clientId,
            CancellationToken ct = default);

        Task<Booking?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
        Task<Booking?> GetByPaymentOrderIdAsync(string paymentOrderId, CancellationToken ct = default);
        Task<IEnumerable<Booking>> GetClientBookingsAsync(
            int clientId, string? status, int page, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<Booking>> GetWorkshopBookingsAsync(
            int workshopId, int page, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<Booking>> GetWorkshopActiveJobsAsync(
            int workshopId, CancellationToken ct = default);
    }
}
