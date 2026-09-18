using FluentValidation;

namespace Application.Features.CarCatalog.Commands.AddCarBrand
{
    public class AddCarBrandCommandValidator : AbstractValidator<AddCarBrandCommand>
    {
        public AddCarBrandCommandValidator()
        {
            RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Brand name is required")
           .MaximumLength(50).WithMessage("Brand name too long");
        }
    }
}
