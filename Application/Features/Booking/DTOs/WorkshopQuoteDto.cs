namespace Application.Features.Booking.DTOs
{
    public record WorkshopQuoteDto(
        int QuoteId,
        int BookingId,
        string BookingNumber,
        string ClientName,
        decimal ExtraTotal,
        string Status,
        DateTime CreatedAt);
}
