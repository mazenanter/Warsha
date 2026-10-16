using Domain.Common;

namespace Domain.Events
{
    public record BookingRejectedByWorkshopEvent(
      int BookingId,
      int ClientId,
      int WorkshopId,
      bool PenaltyApplied,
      decimal PenaltyAmount
  ) : IDomainEvent;
}
