using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.RejectBooking
{
    public record RejectBookingCommand(int BookingId, string? Reason) : IRequest<Result>;
}
