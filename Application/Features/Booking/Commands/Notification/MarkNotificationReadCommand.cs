using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.Notification
{
    public record MarkNotificationReadCommand(int NotificationId) : IRequest<Result>;

}
