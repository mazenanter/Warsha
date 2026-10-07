using Domain.Common;

namespace Domain.Entities
{
    public class Car : BaseEntity
    {
        public int ClientId { get;private set; }
        public Client Client { get;private set; }=null!;
     
        public int Year { get; private set; }

        public int CarModelId { get; private set; }
        public CarModel CarModel { get; private set; } = default!;

        public string? LicensePlate { get; private set; }

        public int ActualOdometerKm { get; private set; }

        public decimal GpsAccumulatedKm { get; private set; }

        public DateTime? GpsMileageLastUpdatedAt { get; private set; }

        private readonly List<OdometerCorrection> _corrections = [];
        public IReadOnlyCollection<OdometerCorrection> Corrections => _corrections;

        protected Car() { }

        public static Car Create(
            int clientId,
            int carModelId,
            int year,
            string licensePlate,
            int actualOdometerKm = 0)
        {
            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("Invalid car year");

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new DomainException("License plate is required");

            if (actualOdometerKm < 0)
                throw new DomainException("Odometer cannot be negative");

            return new Car
            {
                ClientId = clientId,
                CarModelId = carModelId,
                Year = year,
                LicensePlate = licensePlate.Trim().ToUpper(),
                ActualOdometerKm = actualOdometerKm,

                GpsAccumulatedKm = 0
            };
        }

        public void UpdateCar(
            int carModelId,
            int year,
            string? licensePlate)
        {
            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("Invalid car year");

            CarModelId = carModelId;
            Year = year;
            LicensePlate = licensePlate;

            UpdatedAt = DateTime.UtcNow;
        }

        public void AddGpsMileage(decimal distanceKm)
        {
            if (distanceKm <= 0)
                throw new DomainException("Distance must be positive");

            GpsAccumulatedKm += distanceKm;

            GpsMileageLastUpdatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public OdometerCorrection CorrectOdometer(
     int newOdometerKm,
     string? note)
        {
            if (newOdometerKm < 0)
                throw new DomainException("Odometer cannot be negative");

            if (newOdometerKm < ActualOdometerKm)
                throw new DomainException(
                    $"New odometer ({newOdometerKm} km) cannot be less than current ({ActualOdometerKm} km)");

            var correction = OdometerCorrection.Create(
                Id,
                ActualOdometerKm,
                newOdometerKm,
                note);

            _corrections.Add(correction);

            ActualOdometerKm = newOdometerKm;

            GpsAccumulatedKm = 0;
            GpsMileageLastUpdatedAt = DateTime.UtcNow;

            UpdatedAt = DateTime.UtcNow;

            return correction;
        }
        public decimal GetEstimatedCurrentMileage()
        {
            return ActualOdometerKm + GpsAccumulatedKm;
        }
    }
}

