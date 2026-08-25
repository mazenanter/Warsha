using FluentValidation;

namespace Application.Features.Auth.Commands.WorkshopLogin
{
    public class WorkshopLoginCommandValidator : AbstractValidator<WorkshopLoginCommand>
    {
        public WorkshopLoginCommandValidator()
        {
            RuleFor(x=>x.UserName).NotEmpty().WithMessage("UserName is required.");
            RuleFor(x => x.Password)
                 .NotEmpty()
                 .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}
