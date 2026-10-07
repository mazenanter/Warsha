using Domain.Common;
using MediatR;

namespace Application.Features.Client.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}