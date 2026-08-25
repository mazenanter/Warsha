using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Specializations.RemoveSpecialization
{
    public class RemoveWorkshopSpecializationCommandHandler : IRequestHandler<RemoveWorkshopSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveWorkshopSpecializationCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public RemoveWorkshopSpecializationCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveWorkshopSpecializationCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(RemoveWorkshopSpecializationCommand request, CancellationToken cancellationToken)
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
            workshop.RemoveSpecialization(request.SpecializationId);
            await _unitOfWork.Workshops.UpdateAsync(workshop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Specializaion removed successfully from workshop");
            return Result.Success("Specializaion removed successfully");
        }
    }
}
