namespace Application.Features.Workshop.DTOs
{
    public class RevenueChartResponseDto
    {
        public List<RevenueDayDto> Days { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class RevenueDayDto
    {
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
    }
}