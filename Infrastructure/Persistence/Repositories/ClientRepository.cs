using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ClientRepository
    : Repository<Client>, IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Client?> GetByReferralCodeAsync(
            string referralCode,
            CancellationToken ct = default)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(
                    c => c.ReferralCode == referralCode,
                    ct);
        }
    }
}
