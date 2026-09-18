using Domain.Common;

namespace Domain.Entities
{
    public class CarBrand : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Icon { get; private set; }
        private readonly List<CarModel> _models = [];

        public IReadOnlyCollection<CarModel> CarModels => _models;
        protected CarBrand() { }
        public static CarBrand Create(string name, string? icon = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Brand name is required");

            return new CarBrand { Name = name, Icon = icon };
        }
    }
}
