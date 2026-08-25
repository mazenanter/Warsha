using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Services.DeleteWorkshopService
{
    public class DeleteWorkshopServiceCommand : IRequest<Result>
    {
        public int WorkshopServiceId { get; set; }
    }
}
