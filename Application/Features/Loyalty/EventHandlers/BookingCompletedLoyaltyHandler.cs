using Application.Features.Loyalty.Commands.EarnPoints;
using Application.Features.Loyalty.Commands.MakePointsAvailable;
using Application.Interfaces;
using Domain.Enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Loyalty.EventHandlers
{
    public class BookingCompletedLoyaltyHandler
    : INotificationHandler<BookingCompletedEvent>
    {
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public BookingCompletedLoyaltyHandler(ISender sender, IUnitOfWork unitOfWork)
        {
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            BookingCompletedEvent notification, CancellationToken ct)
        {
            await _sender.Send(new EarnPointsCommand(
                notification.ClientId,
                LoyaltyAction.CompleteBooking,
                "Booking completed",
                notification.BookingId), ct);

            var completedCount = await _unitOfWork.Bookings
                .CountCompletedByClientAsync(notification.ClientId, ct);

            if (completedCount == 1)
            {
                await _sender.Send(new EarnPointsCommand(
                    notification.ClientId,
                    LoyaltyAction.FirstBooking,
                    "First booking bonus!",
                    notification.BookingId), ct);
            }

            if (completedCount == 1)
            {
                var client = await _unitOfWork.Clients
                    .GetByIdAsync(notification.ClientId, ct);

                if (client?.ReferredByCode != null)
                {
                    var referrer = await _unitOfWork.Clients
                        .GetByReferralCodeAsync(client.ReferredByCode, ct);

                    if (referrer != null)
                    {
                        await _sender.Send(new EarnPointsCommand(
                            referrer.Id,
                            LoyaltyAction.ReferFriend,
                            $"Referral reward — {client.Name} completed first booking",
                            notification.BookingId), ct);

                        await _sender.Send(new EarnPointsCommand(
                            notification.ClientId,
                            LoyaltyAction.FriendFirstBooking,
                            "Welcome referral bonus",
                            notification.BookingId), ct);
                    }
                }
            }

            await _sender.Send(
                new MakePointsAvailableCommand(notification.BookingId), ct);
        }
    }
}
