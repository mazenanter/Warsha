using FluentValidation;

namespace Application.Features.Client.Reminders.Commands.AddReminder
{
    public class AddReminderCommandValidator : AbstractValidator<AddReminderCommand>
    {
        public AddReminderCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Due date cannot be in the past");
            RuleFor(x => x.CarId).GreaterThan(0).When(x => x.CarId.HasValue);
        }
    }
}