using FluentValidation;

namespace Application.Features.Client.Workshops.Commands.UnsaveWorkshop
{
    public class UnsaveWorkshopCommandValidator : AbstractValidator<UnsaveWorkshopCommand>
    {
        public UnsaveWorkshopCommandValidator()
        {
            RuleFor(x => x.WorkshopId).GreaterThan(0);
        }
    }
}