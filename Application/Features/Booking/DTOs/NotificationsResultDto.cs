namespace Application.Features.Booking.DTOs
{

    public record NotificationsResultDto(
        IEnumerable<NotificationDto> Notifications,
        int UnreadCount);
}
