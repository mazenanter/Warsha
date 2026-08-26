using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeding
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            foreach (var (name, code, module) in Permissions.GetAll())
            {
                var exists = await context.Permissions
                    .AnyAsync(p => p.Name == name);

                if (!exists)
                    await context.Permissions.AddAsync(
                        Permission.Create(name, code, module));
            }

            await context.SaveChangesAsync();
        }
    }
}
