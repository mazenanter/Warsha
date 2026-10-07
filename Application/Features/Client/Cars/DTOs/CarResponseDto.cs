namespace Application.Features.Client.Cars.DTOs
{
    public class CarResponseDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int? ActualOdometerKm { get; set; }
    }
}
