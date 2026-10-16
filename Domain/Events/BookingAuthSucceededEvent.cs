using Domain.Common;

namespace Domain.Events
{
    public record BookingAuthSucceededEvent(
      int BookingId,
      int WorkshopId,
      int ClientId,
      string AuthorizationId
  ) : IDomainEvent;
}
