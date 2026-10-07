namespace Application.Features.Booking.DTOs
{
    public record BookingListDto(
    int Id,
    string BookingNumber,
    string WorkshopName,
    string ServiceNames,
    DateTime ScheduledAt,
    decimal TotalAmount,
    string BookingStatus,
    string JobStatus,
    string PaymentType);
}
