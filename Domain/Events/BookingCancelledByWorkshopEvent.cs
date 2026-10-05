using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public record BookingCancelledByWorkshopEvent(
     int BookingId,
     int ClientId,
     int WorkshopId,
       decimal RefundAmount,
     PaymentType PaymentType
 ) : IDomainEvent;
}
