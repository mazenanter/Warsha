using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.UpdateSettings
{
    public class UpdateWorkshopSettingsCommandHandler : IRequestHandler<UpdateWorkshopSettingsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateWorkshopSettingsCommandHandler> _logger;

        public UpdateWorkshopSettingsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateWorkshopSettingsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateWorkshopSettingsCommand request, CancellationToken cancellationToken)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId);
            if(workshop is null)
            {
                _logger.LogWarning("Workshop with ID {WorkshopId} not found.", request.WorkshopId);
                return Result.Failure($"Workshop with ID {request.WorkshopId} not found.");
            }
            if (!workshop.IsVerified)
            {
                _logger.LogWarning("Workshop with ID {WorkshopId} is not verified.", request.WorkshopId);
                return Result.Failure($"Workshop with ID {request.WorkshopId} is not verified.");
            }
            workshop.UpdateSettings(request.AcceptOnlineBookings, request.ShowPricesToCustomers, request.AutoSendUpdates, request.EmailDailySummary);
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Workshop settings updated successfully for Workshop ID {WorkshopId}.", request.WorkshopId);
            return Result.Success("Workshop settings updated successfully.");
        }
    }
}
