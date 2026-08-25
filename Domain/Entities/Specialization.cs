using Domain.Common;

namespace Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Icon { get; private set; }
        public bool IsActive { get; private set; } = true;
        protected Specialization() { }

        public static Specialization Create(string name, string? icon)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Specialization name is required.");

            return new Specialization
            {
                Name = name,
                Icon = icon
            };
        }

        public void Update(string name, string? icon)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Specialization name is required.");
            Name = name;
            Icon = icon;
            UpdatedAt = DateTime.UtcNow;

        }
        public void InActive()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Active()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
