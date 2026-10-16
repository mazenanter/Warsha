
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Booking.EventHandlers;

public sealed class BookingAuthSucceededHandler
    : INotificationHandler<BookingAuthSucceededEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BookingAuthSucceededHandler> _logger;

    public BookingAuthSucceededHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<BookingAuthSucceededHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(
        BookingAuthSucceededEvent notification,
        CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings
            .GetByIdWithDetailsAsync(
                notification.BookingId,
                cancellationToken);

        if (booking is null)
        {
            _logger.LogWarning(
                "Booking {BookingId} not found for authorization success event.",
                notification.BookingId);

            return;
        }

        if (booking.Workshop.UserId <= 0)
        {
            _logger.LogWarning(
                "Workshop user not found for booking {BookingId}.",
                booking.Id);

            return;
        }

        await _notificationService.SendAsync(
            booking.Workshop.UserId,
            "New Booking Request",
            $"Booking {booking.BookingNumber} is awaiting your approval.",
            new Dictionary<string, string>
            {
                ["bookingId"] = booking.Id.ToString(),
                ["type"] = "WorkshopNewBooking"
            },
            cancellationToken);
    }
}
