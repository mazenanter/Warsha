using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public record QuoteRespondedEvent(
     int BookingId,
     int WorkshopId,
     int QuoteId,
     QuoteStatus Decision
 ) : IDomainEvent;
}
