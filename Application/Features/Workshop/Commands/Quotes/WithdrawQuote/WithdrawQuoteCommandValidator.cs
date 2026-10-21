using FluentValidation;

namespace Application.Features.Workshop.Commands.Quotes.WithdrawQuote
{
    public class WithdrawQuoteCommandValidator : AbstractValidator<WithdrawQuoteCommand>
    {
        public WithdrawQuoteCommandValidator()
        {
            RuleFor(x => x.QuoteId)
                .GreaterThan(0)
                .WithMessage("QuoteId must be a positive integer");
        }
    }
}