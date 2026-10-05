namespace Application.Features.Booking.DTOs
{
    public record PendingQuoteDto(
     int QuoteId,
     decimal ExtraTotal,
     string? Note,
     List<QuoteItemDto> Items);
}
