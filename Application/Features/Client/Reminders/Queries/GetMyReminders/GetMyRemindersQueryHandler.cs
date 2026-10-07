using Application.Features.Client.Reminders.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Reminders.Queries.GetMyReminders
{
    public class GetMyRemindersQueryHandler : IRequestHandler<GetMyRemindersQuery, Result<PagedResult<ReminderResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyRemindersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<ReminderResponseDto>>> Handle(GetMyRemindersQuery request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result<PagedResult<ReminderResponseDto>>.Failure("Client not found in token");

            var urgentThreshold = DateTime.UtcNow.Date.AddDays(3);

            var query = _unitOfWork.Reminders.GetAll()
                .Include(r => r.Car)
                    .ThenInclude(c => c!.CarModel)
                        .ThenInclude(cm => cm.CarBrand)
                .Where(r => r.ClientId == clientId.Value);

            query = request.Filter switch
            {
                ReminderFilter.Completed => query.Where(r => r.IsCompleted),
                ReminderFilter.Urgent => query.Where(r => !r.IsCompleted && r.DueDate <= urgentThreshold),
                ReminderFilter.Upcoming => query.Where(r => !r.IsCompleted && r.DueDate > urgentThreshold),
                _ => query // All
            };

            query = request.Filter == ReminderFilter.Completed
                ? query.OrderByDescending(r => r.CompletedAt)
                : query.OrderBy(r => r.DueDate);

            var totalRecords = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReminderResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Notes = r.Notes,
                    DueDate = r.DueDate,
                    IsCompleted = r.IsCompleted,
                    CompletedAt = r.CompletedAt,
                    IsUrgent = !r.IsCompleted && r.DueDate <= urgentThreshold,
                    CarId = r.CarId,
                    CarDisplayName = r.Car == null ? null : r.Car.CarModel.CarBrand.Name + " " + r.Car.CarModel.Name
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<ReminderResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<ReminderResponseDto>>.Success(pagedResult, "Reminders retrieved successfully");
        }
    }
}