using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Services.ToggleServiceVisibility
{
    public class ToggleServiceVisibilityCommandHandler : IRequestHandler<ToggleServiceVisibilityCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ToggleServiceVisibilityCommandHandler> _logger;

        public ToggleServiceVisibilityCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleServiceVisibilityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(ToggleServiceVisibilityCommand request, CancellationToken cancellationToken)
        {
            var workshopService = await _unitOfWork.WorkshopServices.GetByIdAsync(request.WorkshopServiceId);
            if (workshopService is null)
            {
                _logger.LogWarning("Workshop service with id: {WorkshopServiceId} not found", request.WorkshopServiceId);
                return Result.Failure("Service not found");
            }
            workshopService.ToggleVisiblity();
            await _unitOfWork.WorkshopServices.UpdateAsync(workshopService);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Workshop visiblity updated successfully");
            return Result.Success("Workshop visiblity updated successfully");
        }
    }
}
