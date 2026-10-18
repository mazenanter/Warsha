namespace Application.Features.Client.Workshops.DTOs
{
    public class WorkshopOfferResponseDto
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        // true = the offer covers every service of the workshop
        public bool AppliesToAllServices { get; set; }

        // Only filled for service-specific offers
        public int? ServiceId { get; set; }
        public string? ServiceNameEn { get; set; }
        public string? ServiceNameAr { get; set; }
    }
}