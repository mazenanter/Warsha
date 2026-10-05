using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.WorkshopBusy
{
    public record SetWorkshopBusyCommand(DateTime BusyFrom, DateTime BusyUntil) : IRequest<Result>;

}
