namespace Application.Features.Client.Workshops.DTOs
{
    public class WorkshopServiceResponseDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public string Category { get; set; } = default!;
        public int Duration { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool IsActive { get; set; }
    }
}