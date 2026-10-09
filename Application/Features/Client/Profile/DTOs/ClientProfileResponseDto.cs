namespace Application.Features.Client.Profile.DTOs
{
    public class ClientProfileResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}