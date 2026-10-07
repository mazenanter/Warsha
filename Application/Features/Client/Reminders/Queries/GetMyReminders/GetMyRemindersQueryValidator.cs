using FluentValidation;

namespace Application.Features.Client.Reminders.Queries.GetMyReminders
{
    public class GetMyRemindersQueryValidator : AbstractValidator<GetMyRemindersQuery>
    {
        public GetMyRemindersQueryValidator()
        {
            RuleFor(x => x.Filter).IsInEnum();
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        }
    }
}