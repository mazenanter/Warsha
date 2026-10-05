namespace Application.Features.Booking.DTOs
{
    public record BookingDetailsDto(
    int Id,
    string BookingNumber,
    string WorkshopName,
    string WorkshopAddress,
    string CarBrand,
    string CarModel,
    int CarYear,
    DateTime ScheduledAt,
    decimal TotalAmount,
    decimal ConfirmationFeeAmount,
    string BookingStatus,
    string JobStatus,
    string PaymentType,
    string? CustomerNotes,
    string? CancellationReason,
    DateTime? ConfirmedAt,
    DateTime? CompletedAt,
    List<BookingServiceDto> Services,
    PendingQuoteDto? PendingQuote);
}
