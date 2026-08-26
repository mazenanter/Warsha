using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<ApplicationUser> userManager,
             AppDbContext context,
            IConfiguration configuration)
        {
            await RoleSeeder.SeedAsync(roleManager);
            await PermissionSeeder.SeedAsync(context);
            await SuperAdminSeeder.SeedAsync(userManager, configuration);
        }
    }
}
