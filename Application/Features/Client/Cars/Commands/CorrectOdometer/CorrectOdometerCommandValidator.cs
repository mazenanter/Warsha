using FluentValidation;

namespace Application.Features.Client.Cars.Commands.CorrectOdometer
{
    public class CorrectOdometerCommandValidator : AbstractValidator<CorrectOdometerCommand>
    {
        public CorrectOdometerCommandValidator()
        {
            RuleFor(x => x.CarId).GreaterThan(0);

            RuleFor(x => x.OdometerKm)
                .GreaterThanOrEqualTo(0).WithMessage("Odometer cannot be negative")
                .LessThanOrEqualTo(2_000_000).WithMessage("Odometer value seems unrealistic");

            RuleFor(x => x.Note)
                .MaximumLength(300).When(x => x.Note != null);
        }
    }
}
