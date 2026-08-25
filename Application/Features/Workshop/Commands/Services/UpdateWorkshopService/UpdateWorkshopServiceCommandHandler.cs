using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Services.UpdateWorkshopService
{
    public class UpdateWorkshopServiceCommandHandler : IRequestHandler<UpdateWorkshopServiceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateWorkshopServiceCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UpdateWorkshopServiceCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateWorkshopServiceCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateWorkshopServiceCommand request, CancellationToken cancellationToken)
        {
            var workshopService = await _unitOfWork.WorkshopServices.GetByIdAsync(request.WorkshopServiceId);
            if(workshopService is null)
            {
                _logger.LogWarning("Workshop service with id: {WorkshopServiceId} not found", request.WorkshopServiceId);
                return Result.Failure("Service not found");
            }
            var serviceCategory = await _unitOfWork.ServiceCategories.GetByIdAsync(request.ServiceCategoryId);
            if(serviceCategory is null)
            {
                _logger.LogWarning("Service Category with id: {ServiceCategoryId} not found", request.ServiceCategoryId);
                return Result.Failure($"Service Category with id: {request.ServiceCategoryId} not found");
            }
            var workshopId = _currentUserService.WorkshopId;
            var nameExist = await _unitOfWork.WorkshopServices.FindAsync(x => (x.NameEn == request.NameEn && x.WorkshopId == workshopId) || x.NameAr == request.NameAr && x.WorkshopId == workshopId);
            if(nameExist is not null)
            {
                _logger.LogWarning("Workshop service with this name is already exist");
                return Result.Failure("Service with this name is already exist");
            }
            workshopService.UpdateData(request.NameEn,request.NameAr, request.MinPrice, request.MaxPrice, request.DescriptionEn,request.DescriptionAr, request.Duration, request.ServiceCategoryId);
            await _unitOfWork.WorkshopServices.UpdateAsync(workshopService);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Service updated successfully");
            return Result.Success("Service updated successfully");
        }
    }
}
