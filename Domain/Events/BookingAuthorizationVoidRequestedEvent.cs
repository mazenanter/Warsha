using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public record BookingAuthorizationVoidRequestedEvent(
        int BookingId,
        string AuthorizationId
    ) : IDomainEvent;
}
