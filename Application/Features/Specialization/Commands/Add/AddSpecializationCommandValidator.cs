using FluentValidation;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommandValidator : AbstractValidator<AddSpecializationCommand>
    {
        public AddSpecializationCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
        }
    }
}
