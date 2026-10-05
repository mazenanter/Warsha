using Domain.Common;
using Domain.Entities;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        private readonly IPublisher _publisher;

        public DbSet<Client> Clients { get; set; }
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<WorkshopService> WorkshopServices { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<PlatformFeeConfig> PlatformFeeConfigs { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<WorkshopSubscription> WorkshopSubscriptions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingService> BookingItems { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<BookingFee> BookingFees { get; set; }
        public DbSet<LoyaltyWallet> LoyaltyWallets { get; set; }
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public DbSet<LoyaltyReward> LoyaltyRewards { get; set; }
        public DbSet<RewardVoucher> RewardVouchers { get; set; }
        public DbSet<LoyaltyPointRule> LoyaltyPointRules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public DbSet<LoyaltyExpirationConfig> LoyaltyExpirationConfigs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher) : base(options)
        {
            _publisher = publisher;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var result = await base.SaveChangesAsync(ct);
            await DispatchDomainEventsAsync();
            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            var aggregates = ChangeTracker
                .Entries<BaseAggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var events = aggregates.SelectMany(e => e.DomainEvents).ToList();
            aggregates.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in events)
                await _publisher.Publish(domainEvent);
        }

    }
}
