using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Notification>> GetByRecipientAsync(
        int userId, int page, int pageSize, CancellationToken ct = default)
        => await _context.Notifications
            .Where(n => n.RecipientUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
            => await _context.Notifications
                .CountAsync(n => n.RecipientUserId == userId && !n.IsRead, ct);
    }
}
