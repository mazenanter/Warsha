using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Services.DeleteWorkshopService
{
    public class DeleteWorkshopServiceCommandHandler : IRequestHandler<DeleteWorkshopServiceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteWorkshopServiceCommandHandler> _logger;

        public DeleteWorkshopServiceCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteWorkshopServiceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteWorkshopServiceCommand request, CancellationToken cancellationToken)
        {
            var workshopService = await _unitOfWork.WorkshopServices.GetByIdAsync(request.WorkshopServiceId);
            if (workshopService is null)
            {
                _logger.LogWarning("Workshop service with id: {WorkshopServiceId} not found", request.WorkshopServiceId);
                return Result.Failure("Service not found");
            }
            await _unitOfWork.WorkshopServices.DeleteAsync(workshopService);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Workshop deleted successfully");
            return Result.Success("Workshop deleted successfully");
        }
    }
}
