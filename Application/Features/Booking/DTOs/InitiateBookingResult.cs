namespace Application.Features.Booking.DTOs
{
    public record InitiateBookingResult(
     int BookingId,
     string BookingNumber,
     string? ClientSecret,
     decimal TotalAmount,
     decimal ConfirmationFeeAmount,
     string PaymentType);
}
