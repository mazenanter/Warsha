using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.DeclineQuote
{
    public record DeclineQuoteCommand(int BookingId, int QuoteId) : IRequest<Result>;

}
