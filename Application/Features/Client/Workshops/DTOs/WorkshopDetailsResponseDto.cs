using Domain.Common;

namespace Application.Features.Client.Workshops.DTOs
{
    public class WorkshopDetailsResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double RatingAvg { get; set; }
        public string? GoogleMapsLink { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public TimeOnly StartBusyTime { get;set; } = new TimeOnly(8, 0);
        public TimeOnly BusyDuration { get;set; } = new TimeOnly(8, 0);
        public bool BusyStatus {get; set;}

        public List<WorkshopServiceResponseDto> Services { get; set; } = new();
        public PagedResult<ReviewResponseDto> Reviews { get; set; } = default!;
    }
}