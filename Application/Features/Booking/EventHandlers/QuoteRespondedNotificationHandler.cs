using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class QuoteRespondedNotificationHandler : INotificationHandler<QuoteRespondedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public QuoteRespondedNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(QuoteRespondedEvent notification, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(notification.WorkshopId, ct);
            if (workshop is null) return;

            var (title, body) = notification.Decision == QuoteStatus.Approved
                ? ("Quote Approved ✅", $"Client approved the extra work for booking {booking.BookingNumber}.")
                : ("Quote Declined ❌", $"Client declined the quote for booking {booking.BookingNumber}. Continue with original services.");

            var notif = Notification.Create(
                workshop.UserId, "Workshop",
                NotificationType.QuoteResponded,
                title, body, notification.BookingId);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _notificationService.SendAsync(
                workshop.UserId, title, body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = notification.BookingId.ToString(),
                    ["decision"] = notification.Decision.ToString(),
                    ["type"] = NotificationType.QuoteResponded.ToString()
                }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
