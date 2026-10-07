using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Trip : BaseEntity
    {
        public int CarId { get; private set; }
        public Car Car { get; private set; } = default!;
        public TripStatus Status { get; private set; }
        public TripSource Source { get; private set; }

        public double StartLatitude { get; private set; }
        public double StartLongitude { get; private set; }
        public double? EndLatitude { get; private set; }
        public double? EndLongitude { get; private set; }

        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt { get; private set; }
        public decimal? DistanceKm { get; private set; }

        public int? OBDDeviceId { get; private set; }
        public float? FuelConsumedL { get; private set; }
        public float? AvgSpeedKmh { get; private set; }
        public float? DrivingScore { get; private set; }

        protected Trip() { }


        public static Trip StartGps(int carId, double latitude, double longitude)
        {
            ValidateCoordinates(latitude, longitude);

            return new Trip
            {
                CarId = carId,
                Status = TripStatus.Active,
                Source = TripSource.GPS,
                StartLatitude = latitude,
                StartLongitude = longitude,
                StartedAt = DateTime.UtcNow
            };
        }


        public void Complete(
           decimal distanceKm,
           double endLatitude,
           double endLongitude)
        {
            if (Status != TripStatus.Active)
                throw new DomainException(
                    "Only active trips can be completed");

            ValidateCoordinates(
                endLatitude,
                endLongitude);

            ValidateDistance(distanceKm);

            var endedAt = DateTime.UtcNow;

            ValidateTripDuration(endedAt);
            ValidateSpeedRealism(
                distanceKm,
                endedAt);

            Status = TripStatus.Completed;
            DistanceKm = distanceKm;
            EndLatitude = endLatitude;
            EndLongitude = endLongitude;
            EndedAt = endedAt;
            UpdatedAt = DateTime.UtcNow;
        }


        public void Cancel()
        {
            if (Status == TripStatus.Completed)
                throw new DomainException("Cannot cancel a completed trip");

            Status = TripStatus.Cancelled;
            EndedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOwnedByClient(int clientId) =>
            Car?.ClientId == clientId;


        private static void ValidateCoordinates(double lat, double lng)
        {
            if (lat < -90 || lat > 90)
                throw new DomainException($"Invalid latitude: {lat}. Must be between -90 and 90.");

            if (lng < -180 || lng > 180)
                throw new DomainException($"Invalid longitude: {lng}. Must be between -180 and 180.");
        }

        private static void ValidateDistance(decimal distanceKm)
        {
            if (distanceKm <= 0)
                throw new DomainException("Distance must be greater than 0");

            if (distanceKm > 500)
                throw new DomainException(
                    "Distance exceeds maximum allowed per trip (500 km). " +
                    "Please ensure GPS tracking is working correctly.");
        }

        private void ValidateTripDuration(DateTime endedAt)
        {
            if (endedAt <= StartedAt)
                throw new DomainException("End time must be after start time");

            var duration = endedAt - StartedAt;
            if (duration.TotalSeconds < 30)
                throw new DomainException("Trip duration is too short (minimum 30 seconds)");
        }

        private void ValidateSpeedRealism(decimal distanceKm, DateTime endedAt)
        {
            var durationHours = (endedAt - StartedAt).TotalHours;
            if (durationHours <= 0) return;

            var avgSpeedKmh = (double)distanceKm / durationHours;

            if (avgSpeedKmh > 200)
                throw new DomainException(
                    $"Average speed of {avgSpeedKmh:F0} km/h is unrealistic. " +
                    "Please check distance and timestamps.");
        }
    }
}
