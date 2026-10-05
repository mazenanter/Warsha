using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Booking.EventHandlers
{
    public class JobStatusUpdatedNotificationHandler
    : INotificationHandler<JobStatusUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public JobStatusUpdatedNotificationHandler(
            IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(JobStatusUpdatedEvent notification, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings
                .GetByIdWithDetailsAsync(notification.BookingId, ct);
            if (booking is null) return;

            var clientUser = await _unitOfWork.Clients.GetByIdAsync(notification.ClientId, ct);
            if (clientUser is null) return;

            var (title, body) = notification.NewStatus switch
            {
                JobStatus.Diagnosing => ("🔍 Diagnosis Started", $"Your car at {booking.Workshop.Name} is being diagnosed."),
                JobStatus.InProgress => ("🔧 Work in Progress", $"Work has started on your car at {booking.Workshop.Name}."),
                JobStatus.Ready => ("✅ Car Ready", $"Your car is ready for pickup at {booking.Workshop.Name}!"),
                JobStatus.Completed => ("🎉 Service Completed", $"Your service at {booking.Workshop.Name} is complete. Enjoy!"),
                _ => ("Status Update", $"Booking {booking.BookingNumber} status updated.")
            };

            var notif = Notification.Create(
                clientUser.UserId, "Client",
                NotificationType.JobStatusUpdated,
                title, body, notification.BookingId);

            await _unitOfWork.Notifications.AddAsync(notif, ct);
            await _notificationService.SendAsync(
                clientUser.UserId, title, body,
                new Dictionary<string, string>
                {
                    ["bookingId"] = notification.BookingId.ToString(),
                    ["status"] = notification.NewStatus.ToString(),
                    ["type"] = NotificationType.JobStatusUpdated.ToString()
                }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
