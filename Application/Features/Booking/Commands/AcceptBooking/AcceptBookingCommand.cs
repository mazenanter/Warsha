using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.AcceptBooking
{
    public record AcceptBookingCommand(int BookingId) : IRequest<Result>;
}
