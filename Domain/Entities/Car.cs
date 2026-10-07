using Domain.Common;

namespace Domain.Entities
{
    public class Car : BaseEntity
    {
        public int ClientId { get;private set; }
        public Client Client { get;private set; } = null!;
     
        public int Year { get; private set; }
        public int CarModelId { get; private set; } 
        public CarModel CarModel { get; private set; } = default!;
        public string? LicensePlate { get; private set; } = default!;
        public int? CurrentKm { get; private set; }

        public static Car Create(int clientId, int carModelId, int year, string? licensePlate, int? currentKm)
        {
            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("Invalid car year");

            

            if (currentKm < 0)
                throw new DomainException("KM cannot be negative");

            return new Car
            {
                ClientId = clientId,
                CarModelId = carModelId,
                Year = year,
                LicensePlate = licensePlate,
                CurrentKm = currentKm
            };
        }

        public void UpdateCar(int carModelId, int year, string? licensePlate)
        {
            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("Invalid car year");
            CarModelId = carModelId;
            Year = year;
            LicensePlate = licensePlate;
            UpdatedAt = DateTime.UtcNow;
        }


        public void UpdateKm(int newKm)
        {
            if (newKm < CurrentKm)
                throw new DomainException("New KM cannot be less than current KM");

            CurrentKm = newKm;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
