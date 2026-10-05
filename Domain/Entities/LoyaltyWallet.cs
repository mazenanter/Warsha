using Domain.Common;

namespace Domain.Entities
{
    public class LoyaltyWallet : BaseEntity
    {
        public int ClientId { get; private set; }
        public int AvailablePoints { get; private set; }
        public int PendingPoints { get; private set; }
        public int LifetimeEarned { get; private set; }
        public int LifetimeRedeemed { get; private set; }

        protected LoyaltyWallet() { }

        public static LoyaltyWallet Create(int clientId) =>
            new() { ClientId = clientId };

        public void AddPendingPoints(int points)
        {
            if (points <= 0) throw new DomainException("Points must be positive");
            PendingPoints += points;
            LifetimeEarned += points;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MakeAvailable(int points)
        {
            if (points > PendingPoints)
                throw new DomainException("Insufficient pending points");
            PendingPoints -= points;
            AvailablePoints += points;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeductPoints(int points)
        {
            if (points > AvailablePoints)
                throw new DomainException("Insufficient available points");
            AvailablePoints -= points;
            LifetimeRedeemed += points;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReversePendingPoints(int points)
        {
            var toReverse = Math.Min(points, PendingPoints);
            PendingPoints -= toReverse;
            LifetimeEarned -= toReverse;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReverseAvailablePoints(int points)
        {
            var toReverse = Math.Min(points, AvailablePoints);
            AvailablePoints -= toReverse;
            LifetimeEarned -= toReverse;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ExpirePoints(int points)
        {
            var toExpire = Math.Min(points, AvailablePoints);
            AvailablePoints -= toExpire;
            UpdatedAt = DateTime.UtcNow;
        }

        public int TotalPoints => AvailablePoints + PendingPoints;
    }
}
