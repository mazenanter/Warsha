using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetWorkshopQuotes
{
    public record GetWorkshopQuotesQuery : IRequest<Result<IEnumerable<WorkshopQuoteDto>>>;

}
