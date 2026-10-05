using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public record JobStatusUpdatedEvent(
     int BookingId,
     int ClientId,
     JobStatus NewStatus
 ) : IDomainEvent;
}
