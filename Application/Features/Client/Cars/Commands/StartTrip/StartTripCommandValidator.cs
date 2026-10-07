using FluentValidation;

namespace Application.Features.Client.Cars.Commands.StartTrip
{
    public class StartTripCommandValidator : AbstractValidator<StartTripCommand>
    {
        public StartTripCommandValidator()
        {
            RuleFor(x => x.CarId).GreaterThan(0);

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180");
        }
    }
}
