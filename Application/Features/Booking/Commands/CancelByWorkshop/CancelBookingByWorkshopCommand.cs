using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.CancelByWorkshop
{
    public record CancelBookingByWorkshopCommand(int BookingId, string? Reason) : IRequest<Result>;
}
