namespace Application.Features.Client.Workshops.DTOs
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}