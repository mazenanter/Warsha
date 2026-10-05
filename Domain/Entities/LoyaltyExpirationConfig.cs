using Domain.Common;

namespace Domain.Entities
{
    public class LoyaltyExpirationConfig : BaseEntity
    {
        public int InactivityMonths { get; private set; } = 12;
        public bool IsActive { get; private set; } = true;
        public int NotifyAt30Days { get; private set; } = 30;
        public int NotifyAt7Days { get; private set; } = 7;

        protected LoyaltyExpirationConfig() { }

        public static LoyaltyExpirationConfig Create(int inactivityMonths = 12) =>
            new() { InactivityMonths = inactivityMonths, IsActive = true };

        public void Update(int inactivityMonths, bool isActive)
        {
            if (inactivityMonths < 1)
                throw new DomainException("Inactivity months must be at least 1");

            InactivityMonths = inactivityMonths;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
