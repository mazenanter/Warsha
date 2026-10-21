using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Quotes.WithdrawQuote
{

    public record WithdrawQuoteCommand(int QuoteId) : IRequest<Result>;
}
