using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class BookingRepository
     : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<Booking?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
            => await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Workshop)
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(m => m.CarBrand)
                .Include(b => b.Items)
                .Include(b => b.Quotes)
                    .ThenInclude(q => q.Items)
                .Include(b => b.PaymentTransactions)
                .Include(b => b.Fees)
                .FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task<Booking?> GetByPaymentOrderIdAsync(
            string paymentOrderId, CancellationToken ct = default)
            => await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.PaymentTransactions)
                .Include(b => b.Fees)
                .FirstOrDefaultAsync(b => b.PaymentOrderId == paymentOrderId, ct);

        public async Task<IEnumerable<Booking>> GetClientBookingsAsync(
            int clientId, string? status, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Bookings
                .Include(b => b.Workshop)
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(m => m.CarBrand)
                .Include(b => b.Items)
                .Include(b => b.Quotes)
                .Where(b => b.ClientId == clientId);

            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
                query = query.Where(b => b.BookingStatus == bookingStatus);

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Booking>> GetWorkshopBookingsAsync(
            int workshopId, int page, int pageSize, CancellationToken ct = default)
            => await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(m => m.CarBrand)
                .Include(b => b.Items)
                .Include(b => b.Quotes)
                .Where(b => b.WorkshopId == workshopId &&
                            b.BookingStatus == BookingStatus.Confirmed)
                .OrderByDescending(b => b.ScheduledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task<IEnumerable<Booking>> GetWorkshopActiveJobsAsync(
            int workshopId, CancellationToken ct = default)
            => await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(m => m.CarBrand)
                .Include(b => b.Items)
                .Include(b => b.Quotes)
                .Where(b => b.WorkshopId == workshopId &&
                            b.BookingStatus == BookingStatus.Confirmed &&
                            b.JobStatus != JobStatus.Completed)
                .OrderBy(b => b.ScheduledAt)
                .ToListAsync(ct);

        public async Task<int> CountCompletedByClientAsync(int clientId, CancellationToken ct = default)
            => await _context.Bookings
                .CountAsync(b =>
                    b.ClientId == clientId &&
                    b.BookingStatus == BookingStatus.Completed, ct);
    }
}
