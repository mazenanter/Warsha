using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler
     : IRequestHandler<GetNotificationsQuery, Result<NotificationsResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<NotificationsResultDto>> Handle(
            GetNotificationsQuery request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            var notifs = await _unitOfWork.Notifications
                .GetByRecipientAsync(userId, request.Page, request.PageSize, ct);
            var unreadCount = await _unitOfWork.Notifications.GetUnreadCountAsync(userId, ct);

            var dtos = notifs.Select(n => new NotificationDto(
                n.Id, n.Title, n.Body, n.Type.ToString(),
                n.IsRead, n.BookingId, n.CreatedAt));

            return Result<NotificationsResultDto>.Success(
                new NotificationsResultDto(dtos, unreadCount), "Notifications retrieved successfully");
        }
    }
}
