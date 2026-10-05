using Domain.Common;

namespace Domain.Entities
{
    public class BookingService : BaseEntity
    {
        public int BookingId { get; private set; }
        public Booking Booking { get; private set; } = default!;

        public int WorkshopServiceId { get; private set; }
        public WorkshopService WorkshopService { get; private set; } = default!;
        public string ServiceName { get; set; }

        public decimal Price { get; private set; }

        protected BookingService() { }

        internal static BookingService Create(
           int workshopServiceId, string serviceName, decimal price)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new DomainException("Service name is required");

            if (price <= 0)
                throw new DomainException("Service price must be positive");

            return new BookingService
            {
                WorkshopServiceId = workshopServiceId,
                ServiceName = serviceName,
                Price = price
            };
        }
    }
}
