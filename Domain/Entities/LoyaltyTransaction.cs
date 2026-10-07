using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class LoyaltyTransaction : BaseEntity
    {
        public int ClientId { get; private set; }
        public LoyaltyTransactionType Type { get; private set; }
        public int Amount { get; private set; }
        public int BalanceAfter { get; private set; }
        public LoyaltyTransactionStatus Status { get; private set; }
        public string Reason { get; private set; } = default!;
        public LoyaltyAction? Action { get; private set; }
        public int? BookingId { get; private set; }
        public int? RewardId { get; private set; }
        public int? VoucherId { get; private set; }
        public int? ReferenceTransactionId { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public int? CreatedByAdminId { get; private set; }

        protected LoyaltyTransaction() { }


        public static LoyaltyTransaction CreateEarn(
            int clientId, int points, int balanceAfter,
            LoyaltyAction action, string reason, int? bookingId = null) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.Earn,
                Amount = points,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Pending,
                Reason = reason,
                Action = action,
                BookingId = bookingId
            };

        public static LoyaltyTransaction CreateBonus(
            int clientId, int points, int balanceAfter,
            LoyaltyAction action, string reason, int? bookingId = null) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.Bonus,
                Amount = points,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Pending,
                Reason = reason,
                Action = action,
                BookingId = bookingId
            };

        public static LoyaltyTransaction CreateRedeem(
            int clientId, int points, int balanceAfter,
            int rewardId, string reason) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.Redeem,
                Amount = -points,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Available,
                Reason = reason,
                RewardId = rewardId
            };

        public static LoyaltyTransaction CreateReversal(
            int clientId, int points, int balanceAfter,
            int referenceTransactionId, string reason,
            int? bookingId = null) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.Reversal,
                Amount = -points,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Available,
                Reason = reason,
                ReferenceTransactionId = referenceTransactionId,
                BookingId = bookingId
            };

        public static LoyaltyTransaction CreateManualAdjustment(
            int clientId, int amount, int balanceAfter,
            string reason, int adminId) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.ManualAdjustment,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Available,
                Reason = reason,
                Action = LoyaltyAction.ManualAdjustment,
                CreatedByAdminId = adminId
            };

        public static LoyaltyTransaction CreateExpiration(
            int clientId, int points, int balanceAfter, string reason) =>
            new()
            {
                ClientId = clientId,
                Type = LoyaltyTransactionType.Expiration,
                Amount = -points,
                BalanceAfter = balanceAfter,
                Status = LoyaltyTransactionStatus.Expired,
                Reason = reason
            };


        public void MakeAvailable()
        {
            if (Status != LoyaltyTransactionStatus.Pending)
                throw new DomainException("Transaction is not pending");
            Status = LoyaltyTransactionStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkReversed()
        {
            Status = LoyaltyTransactionStatus.Reversed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetVoucherId(int voucherId) => VoucherId = voucherId;
    }
}
