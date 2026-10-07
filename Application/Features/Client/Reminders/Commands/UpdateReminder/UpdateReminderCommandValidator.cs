using FluentValidation;

namespace Application.Features.Client.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        public UpdateReminderCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.CarId).GreaterThan(0).When(x => x.CarId.HasValue);
            // No lower bound on DueDate here (unlike Add) — a client editing an
            // already-overdue reminder shouldn't be blocked from saving other changes.
        }
    }
}