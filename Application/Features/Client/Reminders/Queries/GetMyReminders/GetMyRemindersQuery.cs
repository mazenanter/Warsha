using Application.Features.Client.Reminders.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Reminders.Queries.GetMyReminders
{
    public class GetMyRemindersQuery : IRequest<Result<PagedResult<ReminderResponseDto>>>
    {
        public ReminderFilter Filter { get; set; } = ReminderFilter.All;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}