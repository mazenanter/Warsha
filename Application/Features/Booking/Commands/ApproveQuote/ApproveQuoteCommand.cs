using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.ApproveQuote
{
    public record ApproveQuoteCommand(int BookingId, int QuoteId) : IRequest<Result>;

}
