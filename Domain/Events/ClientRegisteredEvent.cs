using Domain.Common;

namespace Domain.Events
{
    public record ClientRegisteredEvent(int ClientId) : IDomainEvent;
}
