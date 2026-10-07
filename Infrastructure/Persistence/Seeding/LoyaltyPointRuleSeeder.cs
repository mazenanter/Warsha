using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding
{
    public static class LoyaltyPointRuleSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.LoyaltyPointRules.AnyAsync()) return;

            var rules = new[]
            {
            LoyaltyPointRule.Create(LoyaltyAction.CompleteBooking,    10, isOneTime: false, description: "Complete a booking"),
            LoyaltyPointRule.Create(LoyaltyAction.FirstBooking,       50, isOneTime: true,  description: "First booking bonus"),
            LoyaltyPointRule.Create(LoyaltyAction.CompleteCarProfile, 10, isOneTime: true,  description: "Complete car profile"),
            LoyaltyPointRule.Create(LoyaltyAction.AddMileage,          5, isOneTime: false, frequencyDays: 30, description: "Add vehicle mileage (once per month)"),
            LoyaltyPointRule.Create(LoyaltyAction.LeaveReview,         5, isOneTime: false, description: "Leave a verified review"),
            LoyaltyPointRule.Create(LoyaltyAction.ReferFriend,        50, isOneTime: false, description: "Refer a friend (after their first booking)"),
            LoyaltyPointRule.Create(LoyaltyAction.FriendFirstBooking, 20, isOneTime: true,  description: "Welcome bonus from referral"),
            LoyaltyPointRule.Create(LoyaltyAction.CompleteMaintenance, 5, isOneTime: false, description: "Complete a Warsha-generated reminder"),
        };

            await context.LoyaltyPointRules.AddRangeAsync(rules);
            await context.SaveChangesAsync();
        }
    }
}
