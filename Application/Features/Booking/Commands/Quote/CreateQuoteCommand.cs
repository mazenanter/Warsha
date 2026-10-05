using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.Quote
{
    public record QuoteItemRequest(string Description, decimal Price);

    public record CreateQuoteCommand(
        int BookingId,
        List<QuoteItemRequest> Items,
        string? Note
    ) : IRequest<Result<int>>;
}
