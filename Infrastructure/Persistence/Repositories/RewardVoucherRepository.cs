using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class RewardVoucherRepository
    : Repository<RewardVoucher>, IRewardVoucherRepository
    {
        public RewardVoucherRepository(AppDbContext context) : base(context) { }

        public async Task<RewardVoucher?> GetByCodeAsync(
            string code, CancellationToken ct = default)
            => await _context.RewardVouchers
                .Include(v => v.Reward)
                .FirstOrDefaultAsync(v => v.VoucherCode == code, ct);

        public async Task<IEnumerable<RewardVoucher>> GetByClientIdAsync(
            int clientId, CancellationToken ct = default)
            => await _context.RewardVouchers
                .Include(v => v.Reward)
                .Where(v => v.ClientId == clientId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync(ct);
    }
}
