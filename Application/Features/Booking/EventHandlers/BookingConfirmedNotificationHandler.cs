using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.EventHandlers
{
    public class BookingConfirmedNotificationHandler
    : INotificationHandler<BookingConfirmedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookingConfirmedNotificationHandler> _logger;

        public BookingConfirmedNotificationHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<BookingConfirmedNotificationHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(BookingConfirmedEvent notification, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser != null)
            {
                var clientNotif = Notification.Create(
                    clientUser.UserId, "Client",
                    NotificationType.BookingConfirmed,
                    "Booking Confirmed ✅",
                    $"Your booking {booking.BookingNumber} at {booking.Workshop.Name} is confirmed.",
                    booking.Id);

                await _unitOfWork.Notifications.AddAsync(clientNotif, ct);
                await _notificationService.SendAsync(
                    clientUser.UserId,
                    clientNotif.Title,
                    clientNotif.Body,
                    new Dictionary<string, string>
                    {
                        ["bookingId"] = booking.Id.ToString(),
                        ["type"] = NotificationType.BookingConfirmed.ToString()
                    }, ct);
            }

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(notification.WorkshopId, ct);
            if (workshop != null)
            {
                var workshopNotif = Notification.Create(
                    workshop.UserId, "Workshop",
                    NotificationType.WorkshopNewBooking,
                    "New Booking 🔔",
                    $"You have a new booking #{booking.BookingNumber} scheduled at {booking.ScheduledAt:g}.",
                    booking.Id);

                await _unitOfWork.Notifications.AddAsync(workshopNotif, ct);
                await _notificationService.SendAsync(
                    workshop.UserId,
                    workshopNotif.Title,
                    workshopNotif.Body,
                    new Dictionary<string, string>
                    {
                        ["bookingId"] = booking.Id.ToString(),
                        ["type"] = NotificationType.WorkshopNewBooking.ToString()
                    }, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
