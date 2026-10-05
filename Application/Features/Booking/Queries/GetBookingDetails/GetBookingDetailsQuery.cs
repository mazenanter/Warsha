using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetBookingDetails
{
    public record GetBookingDetailsQuery(int BookingId) : IRequest<Result<BookingDetailsDto>>;

}
