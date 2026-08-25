namespace Application.Features.Workshop.DTOs
{
    public class WorkshopServiceDetailsResponseDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Category { get; set; }
        public int Duration { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public bool IsActive { get; set; }
    }
}
