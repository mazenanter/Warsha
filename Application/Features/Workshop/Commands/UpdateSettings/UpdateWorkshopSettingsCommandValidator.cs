using FluentValidation;

namespace Application.Features.Workshop.Commands.UpdateSettings
{
    public class UpdateWorkshopSettingsCommandValidator :  AbstractValidator<UpdateWorkshopSettingsCommand>
    {
        public UpdateWorkshopSettingsCommandValidator()
        {
            RuleFor(x => x.WorkshopId).GreaterThan(0).WithMessage("Workshop ID must be greater than 0.");

        }
    }
}
