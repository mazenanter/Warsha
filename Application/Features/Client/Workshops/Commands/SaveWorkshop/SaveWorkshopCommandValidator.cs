using FluentValidation;

namespace Application.Features.Client.Workshops.Commands.SaveWorkshop
{
    public class SaveWorkshopCommandValidator : AbstractValidator<SaveWorkshopCommand>
    {
        public SaveWorkshopCommandValidator()
        {
            RuleFor(x => x.WorkshopId).GreaterThan(0);
        }
    }
}