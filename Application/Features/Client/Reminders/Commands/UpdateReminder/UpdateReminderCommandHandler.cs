using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateReminderCommandHandler> _logger;

        public UpdateReminderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateReminderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

            var reminder = await _unitOfWork.Reminders.GetByIdAsync(request.Id);
            if (reminder is null || reminder.ClientId != clientId.Value)
            {
                _logger.LogWarning("Reminder {Id} not found for client {ClientId}", request.Id, clientId.Value);
                return Result.Failure("Reminder not found");
            }

            if (request.CarId.HasValue)
            {
                var car = await _unitOfWork.Cars.GetAll()
                    .FirstOrDefaultAsync(c => c.Id == request.CarId.Value && c.ClientId == clientId.Value, cancellationToken);

                if (car is null)
                {
                    _logger.LogWarning("Car {CarId} not found for client {ClientId}", request.CarId.Value, clientId.Value);
                    return Result.Failure("Car not found");
                }
            }

            reminder.Update(request.Title, request.Notes, request.DueDate, request.CarId);

            if (request.IsCompleted && !reminder.IsCompleted)
                reminder.MarkCompleted();
            else if (!request.IsCompleted && reminder.IsCompleted)
                reminder.MarkIncomplete();

            await _unitOfWork.Reminders.UpdateAsync(reminder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reminder {Id} updated successfully for client {ClientId}", request.Id, clientId.Value);
            return Result.Success("Reminder updated successfully");
        }
    }
}