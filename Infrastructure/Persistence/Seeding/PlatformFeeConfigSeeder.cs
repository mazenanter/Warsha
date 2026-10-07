using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeding
{
    public static class PlatformFeeConfigSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.PlatformFeeConfigs.AnyAsync()) return;

            var config = PlatformFeeConfig.Create(10,25,10);

            await context.PlatformFeeConfigs.AddAsync(config);
            await context.SaveChangesAsync();
        }
    }
}
