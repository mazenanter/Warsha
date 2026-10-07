using Domain.Common;

namespace Domain.Entities
{
    public class OdometerCorrection : BaseEntity
    {
        public int CarId { get; private set; }
        public int PreviousKm { get; private set; }
        public int NewKm { get; private set; }
        public string? Note { get; private set; }
        public DateTime CorrectedAt { get; private set; }

        protected OdometerCorrection() { }

        public static OdometerCorrection Create(
            int carId, int previousKm, int newKm, string? note) =>
            new()
            {
                CarId = carId,
                PreviousKm = previousKm,
                NewKm = newKm,
                Note = note?.Trim(),
                CorrectedAt = DateTime.UtcNow
            };
    }
}
