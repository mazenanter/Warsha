using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Booking.EventHandlers
{
    public class BookingRejectedNotificationHandler
        : INotificationHandler<BookingRejectedByWorkshopEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public BookingRejectedNotificationHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(
            BookingRejectedByWorkshopEvent notification,
            CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);

            if (booking is null)
                return;

            var client = await _unitOfWork.Clients
                .GetByIdAsync(notification.ClientId, ct);

            if (client is null)
                return;

            var notif = Notification.Create(
                client.UserId,
                "Client",
                NotificationType.BookingRejected,
                "Booking Rejected",
                $"Workshop rejected your booking {booking.BookingNumber}.",
                booking.Id);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _notificationService.SendAsync(
                client.UserId,
                notif.Title,
                notif.Body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = booking.Id.ToString(),
                    ["type"] = NotificationType.BookingRejected.ToString()
                },
                ct);
        }
    }
}
