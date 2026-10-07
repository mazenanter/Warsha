using Domain.Common;

namespace Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Icon { get; private set; }
        public bool IsActive { get; private set; } = true;

        public int? CarBrandId { get; private set; }
        public CarBrand? CarBrand { get; private set; }

        public int? CarModelId { get; private set; }
        public CarModel? CarModel { get; private set; }

        protected Specialization() { }

        public static Specialization Create(string name, string? icon, int? carBrandId = null, int? carModelId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Specialization name is required.");

            return new Specialization
            {
                Name = name,
                Icon = icon,
                CarBrandId = carBrandId,
                CarModelId = carModelId
            };
        }

        public void Update(string name, string? icon, int? carBrandId = null, int? carModelId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Specialization name is required.");

            Name = name;
            Icon = icon;
            CarBrandId = carBrandId;
            CarModelId = carModelId;
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