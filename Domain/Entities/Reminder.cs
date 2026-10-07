using Domain.Common;

namespace Domain.Entities
{
    public class Reminder : BaseEntity
    {
        public int ClientId { get; private set; }
        public Client Client { get; private set; } = default!;

        public int? CarId { get; private set; }
        public Car? Car { get; private set; }

        public string Title { get; private set; } = default!;
        public string? Notes { get; private set; }
        public DateTime DueDate { get; private set; }
        public bool IsCompleted { get; private set; } = false;
        public DateTime? CompletedAt { get; private set; }

        protected Reminder() { }

        public static Reminder Create(int clientId, string title, string? notes, DateTime dueDate, int? carId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Reminder title is required.");

            return new Reminder
            {
                ClientId = clientId,
                CarId = carId,
                Title = title,
                Notes = notes,
                DueDate = dueDate
            };
        }

        public void Update(string title, string? notes, DateTime dueDate, int? carId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Reminder title is required.");

            Title = title;
            Notes = notes;
            DueDate = dueDate;
            CarId = carId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkCompleted()
        {
            if (IsCompleted)
                throw new DomainException("Reminder is already completed.");

            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkIncomplete()
        {
            IsCompleted = false;
            CompletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}