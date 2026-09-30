namespace Application.Features.Client.Workshops.DTOs
{
    public class WorkshopResponseDto
    {
        public int UserId { get;set; }
        public string Name { get;set; } = default!;
        public string Email { get;set; } = default!;
        public string Phone { get;set; } = default!;
        public string Address { get;set; } = default!;
        public double Lat { get;set; } 
        public double Lng { get;set; }
        public double RatingAvg { get;set; }
        public string? GoogleMapsLink { get;set; } = null;
        public TimeOnly OpeningTime { get;set; } = new TimeOnly(8,0);
        public TimeOnly ClosingTime { get;set; } = new TimeOnly(8, 0);
        public TimeOnly StartBusyTime { get;set; } = new TimeOnly(8, 0);
        public TimeOnly BusyDuration { get;set; } = new TimeOnly(8, 0);
        public bool BusyStatus {get; set;}
        public double? DistanceKM {get; set;}

    }
}