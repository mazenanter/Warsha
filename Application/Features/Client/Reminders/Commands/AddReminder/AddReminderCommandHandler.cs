using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Reminders.Commands.AddReminder
{
    public class AddReminderCommandHandler : IRequestHandler<AddReminderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AddReminderCommandHandler> _logger;

        public AddReminderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<AddReminderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddReminderCommand request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result.Failure("Client not found in token");

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

            var reminder = Reminder.Create(clientId.Value, request.Title, request.Notes, request.DueDate, request.CarId);
            await _unitOfWork.Reminders.AddAsync(reminder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reminder added successfully for client {ClientId}", clientId.Value);
            return Result.Success("Reminder added successfully");
        }
    }
}