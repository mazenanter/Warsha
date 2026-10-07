using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteReminderCommandHandler> _logger;

        public DeleteReminderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteReminderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
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

            await _unitOfWork.Reminders.DeleteAsync(reminder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reminder {Id} deleted successfully for client {ClientId}", request.Id, clientId.Value);
            return Result.Success("Reminder deleted successfully");
        }
    }
}