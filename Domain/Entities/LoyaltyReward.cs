using Domain.Common;

namespace Domain.Entities
{
    public class LoyaltyReward : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public int PointsCost { get; private set; }
        public decimal? DiscountAmount { get; private set; }
        public int? ExpiryDays { get; private set; }
        public int? RedemptionLimitPerClient { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? StartAt { get; private set; }
        public DateTime? EndAt { get; private set; }

        protected LoyaltyReward() { }

        public static LoyaltyReward Create(
            string name, string description, int pointsCost,
            decimal? discountAmount = null, int? expiryDays = null,
            int? redemptionLimitPerClient = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Reward name is required");
            if (pointsCost <= 0)
                throw new DomainException("Points cost must be positive");

            return new LoyaltyReward
            {
                Name = name.Trim(),
                Description = description,
                PointsCost = pointsCost,
                DiscountAmount = discountAmount,
                ExpiryDays = expiryDays,
                RedemptionLimitPerClient = redemptionLimitPerClient,
                IsActive = true
            };
        }

        public void Update(
            string name, string description, int pointsCost,
            decimal? discountAmount, int? expiryDays, int? redemptionLimit)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Reward name is required");
            if (pointsCost <= 0)
                throw new DomainException("Points cost must be positive");

            Name = name.Trim();
            Description = description;
            PointsCost = pointsCost;
            DiscountAmount = discountAmount;
            ExpiryDays = expiryDays;
            RedemptionLimitPerClient = redemptionLimit;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

        public bool IsAvailable()
        {
            if (!IsActive) return false;
            if (StartAt.HasValue && DateTime.UtcNow < StartAt) return false;
            if (EndAt.HasValue && DateTime.UtcNow > EndAt) return false;
            return true;
        }
    }
}
