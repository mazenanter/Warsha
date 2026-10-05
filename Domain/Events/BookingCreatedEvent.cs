using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public record BookingCreatedEvent(
     int BookingId,
     int WorkshopId,
     int ClientId,
     string BookingNumber,
     PaymentType PaymentType
 ) : IDomainEvent;
}
