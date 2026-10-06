using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Clients = new ClientRepository(_context);
            Reviews = new Repository<Review>(_context);

            WorkshopServices = new Repository<WorkshopService>(_context);
            ServiceCategories = new Repository<ServiceCategory>(_context);
            Specializations = new Repository<Specialization>(_context);
            Workshops = new WorkshopRepository(_context);
            Permissions = new PermissionRepository(_context);
            LoyaltyPointRules = new LoyaltyPointRuleRepository(_context);
            LoyaltyExpirationConfigs = new LoyaltyExpirationConfigRepository(_context);
            RewardVouchers = new RewardVoucherRepository(_context);
            LoyaltyWallets = new LoyaltyWalletRepository(_context);
            LoyaltyTransactions = new LoyaltyTransactionRepository(_context);
            LoyaltyRewards = new LoyaltyRewardRepository(_context);
            Cars = new CarRepository(_context);
            CarModels = new Repository<CarModel>(_context);
            CarBrands = new Repository<CarBrand>(_context);
            Bookings = new BookingRepository(_context);
            BookingItems = new Repository<BookingService>(_context);
            BookingFees = new Repository<BookingFee>(_context);
            PaymentTransactions = new Repository<PaymentTransaction>(_context);
            Quotes = new Repository<Quote>(_context);
            QuoteItems = new Repository<QuoteItem>(_context);
            PlatformFeeConfigs = new PlatformFeeConfigRepository(_context);
            SubscriptionPlans = new Repository<SubscriptionPlan>(_context);
            WorkshopSubscriptions = new Repository<WorkshopSubscription>(_context);
            PaymentMethods = new Repository<PaymentMethod>(_context);
            DeviceTokens = new DeviceTokenRepository(_context);
            Notifications = new NotificationRepository(_context);
        }

        public IRepository<Review> Reviews { get; }

        public IRepository<WorkshopService> WorkshopServices { get; }
        public IRepository<ServiceCategory> ServiceCategories { get; }
        public IRepository<Specialization> Specializations { get; }
        public IRepository<CarModel> CarModels { get; }
        public IRepository<CarBrand> CarBrands { get; }
        public IRepository<BookingService> BookingItems { get; }
        public IRepository<BookingFee> BookingFees { get; }
        public ILoyaltyWalletRepository LoyaltyWallets { get; }
        public ILoyaltyTransactionRepository LoyaltyTransactions { get; }
        public IBookingRepository Bookings { get; }
        public INotificationRepository Notifications { get; }
        public IDeviceTokenRepository DeviceTokens { get; }
        public IClientRepository Clients { get; }
        public ILoyaltyRewardRepository LoyaltyRewards { get; }
        public IRewardVoucherRepository RewardVouchers { get; }
        public ILoyaltyPointRuleRepository LoyaltyPointRules { get; }
        public ILoyaltyExpirationConfigRepository LoyaltyExpirationConfigs { get; }
        public IPlatformFeeConfigRepository PlatformFeeConfigs { get; }
        public IRepository<SubscriptionPlan> SubscriptionPlans { get; }
        public IRepository<WorkshopSubscription> WorkshopSubscriptions { get; }
        public IRepository<PaymentMethod> PaymentMethods { get; }
        public IRepository<PaymentTransaction> PaymentTransactions { get; }
        public IRepository<Quote> Quotes { get; }
        public IRepository<QuoteItem> QuoteItems { get; }
        public IWorkshopRepository Workshops { get; }
        public ICarRepository Cars { get; }
        public IPermissionRepository Permissions { get; }



        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
        public void Dispose()
        {
            _context.Dispose();

        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
