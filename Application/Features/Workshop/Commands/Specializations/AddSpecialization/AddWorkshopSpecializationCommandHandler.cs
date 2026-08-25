using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Specializations.AddSpecialization
{
    public class AddWorkshopSpecializationCommandHandler : IRequestHandler<AddWorkshopSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddWorkshopSpecializationCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        public AddWorkshopSpecializationCommandHandler(IUnitOfWork unitOfWork, ILogger<AddWorkshopSpecializationCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AddWorkshopSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.SpecializationId);
            if (specialization is null)
            {
                _logger.LogWarning("Specializaion with this id : {SpecializationId} not found", request.SpecializationId);
                return Result.Failure("Specializaion not found");
            }
            var workshopId = _currentUserService.WorkshopId;
            if (workshopId is null)
                return Result.Failure("Workshop not found in token");
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(workshopId.Value);
            workshop.AddSpecialization(request.SpecializationId);
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Specializaion added successfully to workshop");
            return Result.Success("Specializaion added successfully");

        }
    }
}
