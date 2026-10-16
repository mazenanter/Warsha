namespace Application.Features.Workshop.DTOs
{
    public class WorkshopProfileResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string AddressEn { get; set; }
        public string AddressAr { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
