using Domain.Common;

namespace Domain.Entities
{
    public class CarModel : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public int CarBrandId { get; private set; }
        public CarBrand CarBrand { get; private set; } = default!;
        private readonly List<Car> _cars = [];

        public IReadOnlyCollection<Car> Cars => _cars;

        private CarModel() { }

        public static CarModel Create(string name, int carBrandId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Model name is required");

            return new CarModel { Name = name, CarBrandId = carBrandId };
        }
    }
}
