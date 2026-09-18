using Domain.Common;

namespace Domain.Entities
{
    public class Offer : BaseEntity
    {
        public int WorkshopId { get;private set; }
        public int? WorkshopServiceId { get;private set; }
        public Workshop Workshop { get; private set; } = default!;
        public WorkshopService? WorkshopService { get; private set; }
        public decimal DiscountPercentage { get; private set; }

        public DateTime StartAt { get; private set; }
        public DateTime EndAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        protected Offer() { }

        internal static Offer CreateWorkshopOffer(int workshopId,decimal discountPercentage,DateTime startAt, DateTime endsAt)
        {
            if (discountPercentage < 0)
                throw new DomainException("Discount percentage must be greater than or equal zero");
            return new Offer
            {
                StartAt = startAt,
                EndAt = endsAt,
                WorkshopId = workshopId,
                DiscountPercentage = discountPercentage,

            };
        }
        internal static Offer CreateServiceOffer(int workshopId, int workshopServiceId, decimal discountPercentage,DateTime startsAt,DateTime endsAt)
        {
            if (discountPercentage < 0)
                throw new DomainException("Discount percentage must be greater than or equal zero");
            return new Offer
            {
                WorkshopId = workshopId,
                WorkshopServiceId = workshopServiceId,
                DiscountPercentage = discountPercentage,
                StartAt = startsAt,
                EndAt = endsAt
            };
        }
    }
}
