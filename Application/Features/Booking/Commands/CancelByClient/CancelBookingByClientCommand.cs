using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.CancelBooking
{
    public record CancelBookingByClientCommand(int BookingId, string? Reason) : IRequest<Result>;
}
