using FluentValidation;

namespace Application.Features.Workshop.Commands.Admin.Unverify
{
    public class UnverifyWorkshopCommandValidator : AbstractValidator<UnverifyWorkshopCommand>
    {
        public UnverifyWorkshopCommandValidator()
        {
            RuleFor(x => x.WorkshopId)
                .GreaterThan(0).WithMessage("WorkshopId must be greater than 0.");
        }
    }
}
