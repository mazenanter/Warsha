using Domain.Common;

namespace Domain.Events
{
    public record BookingPaymentFailedEvent(int BookingId, int ClientId) : IDomainEvent;
}
