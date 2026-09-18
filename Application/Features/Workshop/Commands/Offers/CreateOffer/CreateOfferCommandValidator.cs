using FluentValidation;

namespace Application.Features.Workshop.Commands.Offers.CreateOffer
{
    public class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
    {
        public CreateOfferCommandValidator()
        {

            RuleFor(x => x.serviceId)
                .GreaterThan(0)
                .When(x => x.serviceId.HasValue)
                .WithMessage("Service ID must be greater than 0.");

            RuleFor(x => x.discountPercentage)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(x => x.startAt)
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.endAt)
                .NotEmpty()
                .WithMessage("End date is required.");

            RuleFor(x => x)
                .Must(x => x.endAt > x.startAt)
                .WithMessage("End date must be after start date.");
        }
    }
}
