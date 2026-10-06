using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {

        public IRepository<Review> Reviews { get;}
        public IWorkshopRepository Workshops { get;}
        public ICarRepository Cars { get;}
        public IRepository<WorkshopService> WorkshopServices { get;}
        public IRepository<ServiceCategory> ServiceCategories { get;}
        public IRepository<CarBrand> CarBrands { get;}
        IBookingRepository Bookings { get; }
        IClientRepository Clients { get; }
        INotificationRepository Notifications { get; }
        IDeviceTokenRepository DeviceTokens { get; }
        IPlatformFeeConfigRepository PlatformFeeConfigs { get; }
        public IRepository<BookingFee> BookingFees { get;}
        public IRepository<PaymentTransaction> PaymentTransactions { get;}
        public IRepository<Quote> Quotes { get;}
        public IRepository<QuoteItem> QuoteItems { get;}
        ILoyaltyWalletRepository LoyaltyWallets { get; }
        ILoyaltyTransactionRepository LoyaltyTransactions { get; }
        ILoyaltyRewardRepository LoyaltyRewards { get; }
        IRewardVoucherRepository RewardVouchers { get; }
        ILoyaltyPointRuleRepository LoyaltyPointRules { get; }
        ILoyaltyExpirationConfigRepository LoyaltyExpirationConfigs { get; }
        public IRepository<CarModel> CarModels { get;}
        public IRepository<PaymentMethod> PaymentMethods { get;}
        public IRepository<SubscriptionPlan> SubscriptionPlans { get;}
        public IRepository<WorkshopSubscription> WorkshopSubscriptions { get;}
        public IRepository<BookingService> BookingItems { get;}
        public IRepository<Specialization> Specializations { get;}
        public IPermissionRepository Permissions { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
