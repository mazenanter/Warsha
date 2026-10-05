using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class BookingCancelledByWorkshopNotificationHandler
    : INotificationHandler<BookingCancelledByWorkshopEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public BookingCancelledByWorkshopNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(BookingCancelledByWorkshopEvent notification, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser is null) return;

            var notif = Notification.Create(
                clientUser.UserId, "Client",
                NotificationType.BookingCancelled,
                "Booking Cancelled by Workshop",
                $"Booking {booking.BookingNumber} was cancelled by the workshop. Your confirmation fee will be refunded.",
                notification.BookingId);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _notificationService.SendAsync(
                clientUser.UserId, notif.Title, notif.Body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = notification.BookingId.ToString(),
                    ["type"] = NotificationType.BookingCancelled.ToString()
                }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
