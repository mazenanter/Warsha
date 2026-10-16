using Domain.Common;

namespace Domain.Events
{
    public record BookingAcceptedByWorkshopEvent(
    int BookingId,
    int WorkshopId,
    int ClientId,
    decimal TotalAmount,
    decimal CommissionAmount
) : IDomainEvent;
}
