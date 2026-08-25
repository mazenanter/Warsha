using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Client, Roles.Workshop };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }
    }
}
