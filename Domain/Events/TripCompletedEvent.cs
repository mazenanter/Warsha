using Domain.Common;

namespace Domain.Events
{
    public record TripCompletedEvent(
     int TripId,
     int CarId,
     int ClientId,
     decimal DistanceKm
 ) : IDomainEvent;
}
