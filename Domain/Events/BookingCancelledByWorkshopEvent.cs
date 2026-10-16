using Domain.Common;
using Domain.Enums;

namespace Domain.Events
{
    public record BookingCancelledByWorkshopEvent(
       int BookingId,
        int ClientId,
        int WorkshopId,
        decimal RefundAmount,
        PaymentType PaymentType,
        bool PenaltyApplied = false,
        decimal PenaltyAmount = 0
 ) : IDomainEvent;
}
