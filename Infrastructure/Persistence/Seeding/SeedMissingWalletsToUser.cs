using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeding
{
    public static class SeedMissingWalletsToUser
    {
        public static async Task CreateMissingWalletsAsync(AppDbContext context)
        {
            var clientsWithoutWallets = await context.Clients
                .Where(c => !context.LoyaltyWallets.Any(w => w.ClientId == c.Id))
                .ToListAsync();

            foreach (var client in clientsWithoutWallets)
                await context.LoyaltyWallets.AddAsync(LoyaltyWallet.Create(client.Id));

            await context.SaveChangesAsync();
        }
    }
}
