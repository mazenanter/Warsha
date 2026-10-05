using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetWorkshopBookings
{
    public record GetWorkshopBookingsQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<IEnumerable<BookingListDto>>>;
}
