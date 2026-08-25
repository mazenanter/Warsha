namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string name, string otpCode, CancellationToken ct = default);
        Task SendWorkshopApprovedAsync(string email, string workshopName, CancellationToken ct = default);
        Task SendWorkshopRejectedAsync(string email, string workshopName, CancellationToken ct = default);
        Task SendBookingConfirmedAsync(string email, string clientName, string bookingCode, string workshopName, DateTime scheduledAt, CancellationToken ct = default);
        Task SendQuoteReceivedAsync(string email, string clientName, string bookingCode, decimal extraAmount, CancellationToken ct = default);
        Task SendDailySummaryAsync(string email, string workshopName, int bookingsCount, decimal revenue, CancellationToken ct = default);
    }
}
