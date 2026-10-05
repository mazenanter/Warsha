using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class PaymentFailedNotificationHandler
      : INotificationHandler<BookingPaymentFailedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public PaymentFailedNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(BookingPaymentFailedEvent notification, CancellationToken ct)
        {
            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser is null) return;

            var notif = Notification.Create(
                clientUser.UserId, "Client",
                NotificationType.BookingPaymentFailed,
                "Payment Failed ❌",
                "Your payment could not be processed. Please try again or use a different method.",
                notification.BookingId);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _notificationService.SendAsync(
                clientUser.UserId, notif.Title, notif.Body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = notification.BookingId.ToString(),
                    ["type"] = NotificationType.BookingPaymentFailed.ToString()
                }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
