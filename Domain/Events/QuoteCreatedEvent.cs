using Domain.Common;

namespace Domain.Events
{
    public record QuoteCreatedEvent(
     int BookingId,
     int ClientId,
     decimal ExtraAmount
 ) : IDomainEvent;
}
