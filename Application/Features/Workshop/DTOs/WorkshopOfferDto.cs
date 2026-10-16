namespace Application.Features.Workshop.DTOs
{
    public class WorkshopOfferDto
    {
        public int Id { get; set; }
        public int? WorkshopServiceId { get; set; }
        public string? ServiceNameEn { get; set; }
        public string? ServiceNameAr { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool IsActive { get; set; }
    }
}
