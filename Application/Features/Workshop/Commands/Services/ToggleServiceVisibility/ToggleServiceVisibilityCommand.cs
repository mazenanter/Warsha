using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Services.ToggleServiceVisibility
{
    public class ToggleServiceVisibilityCommand : IRequest<Result>
    {
        public int WorkshopServiceId { get; set; }
    }
}
