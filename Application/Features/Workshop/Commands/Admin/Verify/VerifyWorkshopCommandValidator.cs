using FluentValidation;

namespace Application.Features.Workshop.Commands.Admin.Verify
{
    public class VerifyWorkshopCommandValidator : AbstractValidator<VerifyWorkshopCommand>
    {
        public VerifyWorkshopCommandValidator()
        {
            RuleFor(x => x.WorkshopId)
                .GreaterThan(0).WithMessage("WorkshopId must be greater than 0.");
        }
    }
}
