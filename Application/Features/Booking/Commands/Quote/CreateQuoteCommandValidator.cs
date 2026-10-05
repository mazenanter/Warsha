using FluentValidation;

namespace Application.Features.Booking.Commands.Quote
{

    public class CreateQuoteCommandValidator : AbstractValidator<CreateQuoteCommand>
    {
        public CreateQuoteCommandValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
            RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Description).NotEmpty().MaximumLength(200);
                item.RuleFor(i => i.Price).GreaterThan(0);
            });
            RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note != null);
        }
    }
}
