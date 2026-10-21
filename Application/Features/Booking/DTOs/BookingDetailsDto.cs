namespace Application.Features.Booking.DTOs
{
    public record BookingDetailsDto(
    int Id,
    string BookingNumber,
    string WorkshopName,
    string WorkshopAddressEn,
    string WorkshopAddressAr,
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
    PendingQuoteDto? PendingQuote,
    BookingCustomerDto? Customer = null,
    int? NextJobStatus = null);

public record BookingCustomerDto(string Name, string Phone);
}
