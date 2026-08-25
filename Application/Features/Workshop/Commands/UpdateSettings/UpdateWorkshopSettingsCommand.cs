using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.UpdateSettings
{
    public class UpdateWorkshopSettingsCommand : IRequest<Result>
    {
        public int WorkshopId { get; set; }

        public bool AcceptOnlineBookings { get; set; }
        public bool ShowPricesToCustomers { get; set; }
        public bool AutoSendUpdates { get; set; }
        public bool EmailDailySummary { get; set; }
    }
}
