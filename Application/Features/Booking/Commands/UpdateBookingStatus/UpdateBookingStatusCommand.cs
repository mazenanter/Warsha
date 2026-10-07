using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Commands.UpdateBookingStatus
{
    public record UpdateBookingStatusCommand(int BookingId, JobStatus NewStatus) : IRequest<Result>;
}
