using FluentValidation;

namespace Application.Features.Client.Cars.Commands.UpdateCar
{
    public class UpdateCarCommandValidator : AbstractValidator<UpdateCarCommand>
    {
        public UpdateCarCommandValidator()
        {
            RuleFor(x => x.CarId).NotEqual(0).WithMessage("Car ID is required.");
            RuleFor(x => x.CarModel).NotEqual(0).WithMessage("Car model is required.");
            RuleFor(x => x.Year)
                .NotEmpty().WithMessage("Year is required.")
                .InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Invalid year.");
        }
    }
}
