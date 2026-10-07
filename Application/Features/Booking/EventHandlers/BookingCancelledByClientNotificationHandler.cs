using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class BookingCancelledByClientNotificationHandler
     : INotificationHandler<BookingCancelledByClientEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public BookingCancelledByClientNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(BookingCancelledByClientEvent notification, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(notification.WorkshopId, ct);
            if (workshop != null)
            {
                var notif = Notification.Create(
                    workshop.UserId, "Workshop",
                    NotificationType.BookingCancelled,
                    "Booking Cancelled",
                    $"Booking {booking.BookingNumber} was cancelled by the client.",
                    notification.BookingId);

                await _unitOfWork.Notifications.AddAsync(notif, ct);
                await _notificationService.SendAsync(
                    workshop.UserId, notif.Title, notif.Body,
                    new Dictionary<string, string>
                    {
                        ["bookingId"] = notification.BookingId.ToString(),
                        ["type"] = NotificationType.BookingCancelled.ToString()
                    }, ct);
            }

            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser != null)
            {
                var refundMsg =
                    "Your confirmation fee will be refunded within 3-5 business days."
                  ;

                var clientNotif = Notification.Create(
                    clientUser.UserId, "Client",
                    NotificationType.BookingCancelled,
                    "Booking Cancelled",
                    $"Booking {booking.BookingNumber} cancelled. {refundMsg}",
                    notification.BookingId);

                await _unitOfWork.Notifications.AddAsync(clientNotif, ct);
                await _notificationService.SendAsync(
                    clientUser.UserId, clientNotif.Title, clientNotif.Body, null, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
