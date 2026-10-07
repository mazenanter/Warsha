using Domain.Common;
using MediatR;

namespace Application.Features.Client.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime DueDate { get; set; }
        public int? CarId { get; set; }
        public bool IsCompleted { get; set; }
    }
}