using Application.Features.Loyalty.Commands.ReversePoints;
using Domain.Events;
using MediatR;

namespace Application.Features.Loyalty.EventHandlers
{
    public class BookingCancelledLoyaltyHandler
    : INotificationHandler<BookingCancelledByClientEvent>
    {
        private readonly ISender _sender;

        public BookingCancelledLoyaltyHandler(ISender sender)
            => _sender = sender;

        public async Task Handle(
            BookingCancelledByClientEvent notification, CancellationToken ct)
        {
            await _sender.Send(new ReversePointsCommand(
                notification.ClientId,
                notification.BookingId,
                "Booking cancelled"), ct);
        }
    }
}
