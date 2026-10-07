using Domain.Common;

namespace Domain.Events
{
    public record BookingCompletedEvent(
     int BookingId,
     int ClientId,
     int WorkshopId,
     int CarId
 ) : IDomainEvent;
}
