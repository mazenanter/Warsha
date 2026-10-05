using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class RewardVoucher : BaseEntity
    {
        public string VoucherCode { get; private set; } = default!;
        public int ClientId { get; private set; }
        public int RewardId { get; private set; }
        public LoyaltyReward Reward { get; private set; } = default!;
        public int LoyaltyTransactionId { get; private set; }
        public RewardVoucherStatus Status { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public int? UsedAtBookingId { get; private set; }

        protected RewardVoucher() { }

        public static RewardVoucher Create(
            int clientId, int rewardId,
            int transactionId, int? expiryDays = null) =>
            new()
            {
                VoucherCode = GenerateCode(),
                ClientId = clientId,
                RewardId = rewardId,
                LoyaltyTransactionId = transactionId,
                Status = RewardVoucherStatus.Available,
                ExpiresAt = expiryDays.HasValue
                    ? DateTime.UtcNow.AddDays(expiryDays.Value)
                    : null
            };

        public void Use(int bookingId)
        {
            if (Status != RewardVoucherStatus.Available)
                throw new DomainException("Voucher is not available");
            if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt)
                throw new DomainException("Voucher has expired");

            Status = RewardVoucherStatus.Used;
            UsedAt = DateTime.UtcNow;
            UsedAtBookingId = bookingId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Expire()
        {
            if (Status != RewardVoucherStatus.Available) return;
            Status = RewardVoucherStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == RewardVoucherStatus.Used)
                throw new DomainException("Cannot cancel a used voucher");
            Status = RewardVoucherStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());
            return $"WR-{code}";
        }
    }
}
