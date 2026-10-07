using FluentValidation;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommandValidator : AbstractValidator<AddSpecializationCommand>
    {
        public AddSpecializationCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.CarBrandId).GreaterThan(0).When(x => x.CarBrandId.HasValue);
            RuleFor(x => x.CarModelId).GreaterThan(0).When(x => x.CarModelId.HasValue);
        }
    }
}