using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Services.AddWorkshopService
{
    public class AddWorkshopServiceCommandHandler : IRequestHandler<AddWorkshopServiceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddWorkshopServiceCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AddWorkshopServiceCommandHandler(IUnitOfWork unitOfWork, ILogger<AddWorkshopServiceCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AddWorkshopServiceCommand request, CancellationToken cancellationToken)
        {
            var workshopId = _currentUserService.WorkshopId;
            if (workshopId is null)
                return Result.Failure("Workshop not found in token");

            var workshop = await _unitOfWork.Workshops
            .GetByIdWithServicesAsync(workshopId.Value, cancellationToken);

            if (workshop is null)
            {
                _logger.LogWarning("Workshop {WorkshopId} not found", workshopId);
                return Result.Failure("Workshop not found");
            }
            var result = workshop.AddService(
           request.NameEn,request.NameAr, request.MinPrice, request.MaxPrice,
           request.DescriptionEn,request.DescriptionAr, request.Duration, request.ServiceCategoryId);
            if (!result.IsSuccess)
                return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Service {ServiceName} added to Workshop {WorkshopId}",
                request.NameEn, workshopId);

            return Result.Success("Service added successfully");

        }
    }
}
