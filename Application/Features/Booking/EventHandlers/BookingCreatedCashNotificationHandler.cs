using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class BookingCreatedCashNotificationHandler
    : INotificationHandler<BookingCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public BookingCreatedCashNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(BookingCreatedEvent notification, CancellationToken ct)
        {
            if (notification.PaymentType != PaymentType.CashOnDelivery) return;

            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser is null) return;

            var notif = Notification.Create(
                clientUser.UserId, "Client",
                NotificationType.BookingConfirmed,
                "Booking Confirmed (Cash) ✅",
                $"Booking {notification.BookingNumber} confirmed. Pay at the workshop.",
                notification.BookingId);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _notificationService.SendAsync(
                clientUser.UserId, notif.Title, notif.Body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = notification.BookingId.ToString(),
                    ["type"] = NotificationType.BookingConfirmed.ToString()
                }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
