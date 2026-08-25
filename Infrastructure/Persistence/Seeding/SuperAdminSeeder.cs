using Domain.Enums;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Seeding;

public static class SuperAdminSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["SuperAdmin:Email"]!;
        var password = configuration["SuperAdmin:Password"]!;
        var phone = configuration["SuperAdmin:Phone"]!;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return;

        var superAdmin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = phone,
            EmailConfirmed = true,        
            IsActive = true,
            UserType = UserType.SUPERADMIN,
        };

        var result = await userManager.CreateAsync(superAdmin, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"SuperAdmin seeding failed: {errors}");
        }

        await userManager.AddToRoleAsync(superAdmin, "SUPERADMIN");
    }
}