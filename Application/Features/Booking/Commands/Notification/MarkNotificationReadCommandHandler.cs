using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.Notification
{
    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public MarkNotificationReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(MarkNotificationReadCommand command, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            var notification = await _unitOfWork.Notifications.GetByIdAsync(command.NotificationId, ct);

            if (notification is null) return Result.Failure("Notification not found");
            if (notification.RecipientUserId != userId) return Result.Failure("Unauthorized");

            notification.MarkRead();
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Notification marked as read");
        }
    }
}
