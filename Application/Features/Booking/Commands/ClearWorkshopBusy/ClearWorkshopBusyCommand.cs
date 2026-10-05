using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.ClearWorkshopBusy
{
    public record ClearWorkshopBusyCommand : IRequest<Result>;
}
