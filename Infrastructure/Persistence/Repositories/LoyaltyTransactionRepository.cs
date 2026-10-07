using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class LoyaltyTransactionRepository
    : Repository<LoyaltyTransaction>, ILoyaltyTransactionRepository
    {
        public LoyaltyTransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<LoyaltyTransaction>> GetByClientIdAsync(
            int clientId, int page, int pageSize, CancellationToken ct = default)
            => await _context.LoyaltyTransactions
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task<bool> HasEarnedForActionAsync(
            int clientId, LoyaltyAction action, CancellationToken ct = default)
            => await _context.LoyaltyTransactions
                .AnyAsync(t =>
                    t.ClientId == clientId &&
                    t.Action == action &&
                    t.Status != LoyaltyTransactionStatus.Reversed, ct);

        public async Task<LoyaltyTransaction?> GetLastByActionAsync(
            int clientId, LoyaltyAction action, CancellationToken ct = default)
            => await _context.LoyaltyTransactions
                .Where(t =>
                    t.ClientId == clientId &&
                    t.Action == action &&
                    t.Status != LoyaltyTransactionStatus.Reversed)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<LoyaltyTransaction>> GetPendingByBookingAsync(
            int bookingId, CancellationToken ct = default)
            => await _context.LoyaltyTransactions
                  .Where(t =>
            t.BookingId == bookingId &&
            (t.Status == LoyaltyTransactionStatus.Pending ||
             t.Status == LoyaltyTransactionStatus.Available))
        .ToListAsync(ct);

        public async Task<IEnumerable<(int ClientId, int PendingPoints, int AvailablePoints)>>
            GetClientsForExpirationAsync(int inactivityMonths, CancellationToken ct = default)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-inactivityMonths);

            var activeClientIds = await _context.LoyaltyTransactions
                .Where(t => t.CreatedAt >= cutoffDate)
                .Select(t => t.ClientId)
                .Distinct()
                .ToListAsync(ct);

            var walletsToExpire = await _context.LoyaltyWallets
                .Where(w =>
                    !activeClientIds.Contains(w.ClientId) &&
                    (w.AvailablePoints > 0 || w.PendingPoints > 0))
                .Select(w => new
                {
                    w.ClientId,
                    w.PendingPoints,
                    w.AvailablePoints
                })
                .ToListAsync(ct);

            return walletsToExpire
                .Select(w => (w.ClientId, w.PendingPoints, w.AvailablePoints));
        }
    }
}
