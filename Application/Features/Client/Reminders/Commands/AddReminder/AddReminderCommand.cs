using Domain.Common;
using MediatR;

namespace Application.Features.Client.Reminders.Commands.AddReminder
{
    public class AddReminderCommand : IRequest<Result>
    {
        public string Title { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime DueDate { get; set; }
        public int? CarId { get; set; }
    }
}