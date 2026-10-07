using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class LoyaltyPointRule : BaseEntity
    {
        public LoyaltyAction Action { get; private set; }
        public int Points { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsOneTime { get; private set; }
        public int? FrequencyDays { get; private set; }
        public string Description { get; private set; } = default!;

        private static readonly int[] ValidPoints = [5, 10, 20, 50, 100];

        protected LoyaltyPointRule() { }

        public static LoyaltyPointRule Create(
            LoyaltyAction action, int points,
            bool isOneTime = false, int? frequencyDays = null,
            string description = "")
        {
            if (!ValidPoints.Contains(points))
                throw new DomainException("Points must be: 5, 10, 20, 50, or 100");

            return new LoyaltyPointRule
            {
                Action = action,
                Points = points,
                IsActive = true,
                IsOneTime = isOneTime,
                FrequencyDays = frequencyDays,
                Description = description
            };
        }

        public void Update(int points, bool isActive, int? frequencyDays)
        {
            if (!ValidPoints.Contains(points))
                throw new DomainException("Points must be: 5, 10, 20, 50, or 100");

            Points = points;
            IsActive = isActive;
            FrequencyDays = frequencyDays;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
