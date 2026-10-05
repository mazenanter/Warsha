using Domain.Common;

namespace Domain.Entities
{
    public class PlatformFeeConfig : BaseEntity
    {
        public decimal ClientBookingFee { get; private set; }
        public DateTime EffectiveFrom { get; private set; }
        public decimal CommissionPct { get; private set; }  
        public decimal WorkshopCancellationFee { get; private set; }

        protected PlatformFeeConfig() { }

        public static PlatformFeeConfig Create(
         decimal clientBookingFee,
         decimal commissionPct,
         decimal workshopCancellationFee)
        {
            if (clientBookingFee < 0)
                throw new DomainException("Booking fee cannot be negative");
            if (commissionPct < 0 || commissionPct > 100)
                throw new DomainException("Commission must be between 0 and 100");
            if (workshopCancellationFee < 0)
                throw new DomainException("Cancellation fee cannot be negative");

            return new PlatformFeeConfig
            {
                ClientBookingFee = clientBookingFee,
                CommissionPct = commissionPct,
                WorkshopCancellationFee = workshopCancellationFee,
                EffectiveFrom = DateTime.UtcNow
            };
        }

        public void Update(
            decimal clientBookingFee,
            decimal commissionPct,
            decimal workshopCancellationFee)
        {
            if (commissionPct < 0 || commissionPct > 100)
                throw new DomainException("Commission must be between 0 and 100");

            ClientBookingFee = clientBookingFee;
            CommissionPct = commissionPct;
            WorkshopCancellationFee = workshopCancellationFee;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
