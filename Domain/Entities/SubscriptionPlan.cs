using Domain.Common;

namespace Domain.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public decimal MonthlyPrice { get; private set; }
        public int MaxServices { get; private set; }  
        public bool PriorityRanking { get; private set; } 
        public bool AdvancedAnalytics { get; private set; } 
        public bool FeaturedListing { get; private set; }
        public decimal WorkshopCommissionPct { get; private set; }  
        public decimal WorkshopCancellationFee { get; private set; }

        private readonly List<WorkshopSubscription> _subscriptions = [];
        public IReadOnlyCollection<WorkshopSubscription> Subscriptions => _subscriptions;

        protected SubscriptionPlan() { }

        public static SubscriptionPlan Create(
            string name,
            decimal monthlyPrice,
            int dailyJobQuota,
            decimal workshopCommissionPct,
            decimal workshopCancellationFee,
            int maxServices,
            bool priorityRanking,
            bool advancedAnalytics,
            bool featuredListing)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Plan name is required");

            if (monthlyPrice < 0)
                throw new DomainException("Monthly price cannot be negative");

            if (dailyJobQuota <= 0)
                throw new DomainException("Daily job quota must be positive");

            if (workshopCommissionPct < 0 || workshopCommissionPct > 100)
                throw new DomainException("Commission percentage must be between 0 and 100");

            if (workshopCancellationFee < 0)
                throw new DomainException("Cancellation fee cannot be negative");

            return new SubscriptionPlan
            {
                Name = name.Trim(),
                MonthlyPrice = monthlyPrice,
                MaxServices = maxServices,
                PriorityRanking = priorityRanking,
                AdvancedAnalytics = advancedAnalytics,
                FeaturedListing = featuredListing,
                WorkshopCommissionPct = workshopCommissionPct,
                WorkshopCancellationFee = workshopCancellationFee
            };
        }

        public void Update(
            string name,
            decimal monthlyPrice,
            int maxServices,
            bool priorityRanking,
            bool advancedAnalytics,
            bool featuredListing,
            decimal commissionPct,
            decimal cancellationFee)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Plan name is required");

            if (commissionPct < 0 || commissionPct > 100)
                throw new DomainException("Commission must be between 0 and 100");

            Name = name.Trim();
            MonthlyPrice = monthlyPrice;
            MaxServices = maxServices;
            PriorityRanking = priorityRanking;
            AdvancedAnalytics = advancedAnalytics;
            FeaturedListing = featuredListing;
            WorkshopCommissionPct = commissionPct;
            WorkshopCancellationFee = cancellationFee;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
