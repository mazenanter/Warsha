namespace Application.Features.Booking.DTOs
{
    public record JobsBoardDto(
    List<JobCardDto> New,
    List<JobCardDto> Diagnosing,
    List<JobCardDto> InProgress,
    List<JobCardDto> Ready);
}
