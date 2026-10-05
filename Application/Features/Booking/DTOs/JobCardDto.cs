namespace Application.Features.Booking.DTOs
{
    public record JobCardDto(
        int BookingId,
        string BookingNumber,
        string ClientName,
        string CarBrand,
        string CarModel,
        string Services,
        DateTime ScheduledAt,
        bool HasPendingQuote);
}
