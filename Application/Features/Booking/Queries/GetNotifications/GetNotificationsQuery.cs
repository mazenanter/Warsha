using Application.Features.Booking.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetNotifications
{
    public record GetNotificationsQuery(int Page = 1, int PageSize = 20)
      : IRequest<Result<NotificationsResultDto>>;
}
