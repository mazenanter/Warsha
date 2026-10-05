using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding
{
    public static class LoyaltyRewardSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.LoyaltyRewards.AnyAsync()) return;

            var rewards = new[]
            {
            LoyaltyReward.Create("Basic Inspection",       "Free basic car inspection",    100,  discountAmount: null,   expiryDays: 30),
            LoyaltyReward.Create("50 EGP Discount",        "50 EGP off any service",       250,  discountAmount: 50,    expiryDays: 30),
            LoyaltyReward.Create("100 EGP Discount",       "100 EGP off any service",      500,  discountAmount: 100,   expiryDays: 30),
            LoyaltyReward.Create("Oil Change Discount",    "Free oil change service",       750,  discountAmount: null,   expiryDays: 30),
            LoyaltyReward.Create("Free Car Wash",          "Free basic car wash",          1000,  discountAmount: null,   expiryDays: 30),
            LoyaltyReward.Create("200 EGP Discount",       "200 EGP off any service",      1500,  discountAmount: 200,   expiryDays: 30),
            LoyaltyReward.Create("Premium Detailing",      "Free premium car detailing",   2000,  discountAmount: null,   expiryDays: 30),
        };

            await context.LoyaltyRewards.AddRangeAsync(rewards);
            await context.SaveChangesAsync();
        }
    }
}
