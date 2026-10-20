namespace Application.Features.Workshop.DTOs
{
    public class DashboardStatsResponseDto
    {
        public int TodaysBookings { get; set; }
        public int YesterdaysBookings { get; set; }
        public int CarsInService { get; set; }
        public int AwaitingApproval { get; set; }
        public RevenueStatDto RevenueToday { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }

    public class RevenueStatDto
    {
        public decimal Amount { get; set; }

        // null when yesterday's revenue was 0 (no baseline to compare against)
        public decimal? ChangePercent { get; set; }
    }
}