using Application.Features.Booking.DTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Commands.InitiateBooking
{
    public record InitiateBookingCommand(
     int WorkshopId,
     int CarId,
     List<int> ServiceIds,
     DateTime ScheduledAt,
     PaymentType PaymentType,
     string? CustomerNotes
 ) : IRequest<Result<InitiateBookingResult>>;
}
