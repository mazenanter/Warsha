namespace Application.Features.Client.Reminders.DTOs
{
    public class ReminderResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsUrgent { get; set; }

        public int? CarId { get; set; }
        public string? CarDisplayName { get; set; } // e.g. "Toyota Corolla" — null for general reminders
    }
}