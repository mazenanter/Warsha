using FluentValidation;

namespace Application.Features.Client.Cars.Commands.CompleteTrip
{
    public class CompleteTripCommandValidator : AbstractValidator<CompleteTripCommand>
    {
        public CompleteTripCommandValidator()
        {
            RuleFor(x => x.TripId).GreaterThan(0);

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage("Distance must be greater than 0")
                .LessThanOrEqualTo(500).WithMessage("Distance cannot exceed 500 km per trip");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180");

           
        }
    }
}
