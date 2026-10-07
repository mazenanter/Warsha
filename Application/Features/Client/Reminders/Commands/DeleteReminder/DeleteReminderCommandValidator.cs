using FluentValidation;

namespace Application.Features.Client.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommandValidator : AbstractValidator<DeleteReminderCommand>
    {
        public DeleteReminderCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}