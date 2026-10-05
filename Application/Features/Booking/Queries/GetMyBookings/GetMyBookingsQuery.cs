using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetMyBookings
{

    public record GetMyBookingsQuery(string? Status = null, int Page = 1, int PageSize = 10)
        : IRequest<Result<IEnumerable<BookingListDto>>>;
}
