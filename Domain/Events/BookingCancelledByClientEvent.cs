using Domain.Common;

namespace Domain.Events
{
    public record BookingCancelledByClientEvent(
      int BookingId,
      int ClientId,
      int WorkshopId,
      decimal ConfirmationFeeAmount
  ) : IDomainEvent;
}
