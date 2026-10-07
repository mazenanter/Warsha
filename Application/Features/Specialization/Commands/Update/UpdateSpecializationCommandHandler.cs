using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Specialization.Commands.Update
{
    public class UpdateSpecializationCommandHandler : IRequestHandler<UpdateSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateSpecializationCommandHandler> _logger;

        public UpdateSpecializationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateSpecializationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.Id);
            if (specialization is null)
            {
                _logger.LogWarning("Specialization with id: {Id} not found", request.Id);
                return Result.Failure("Specialization not found");
            }

            var nameExists = await _unitOfWork.Specializations.FindAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != request.Id);
            if (nameExists is not null)
            {
                _logger.LogWarning("Specialization with name: {Name} already exist", request.Name);
                return Result.Failure("Specialization already exist");
            }

            if (request.CarBrandId.HasValue)
            {
                var carBrand = await _unitOfWork.CarBrands.GetByIdAsync(request.CarBrandId.Value);
                if (carBrand is null)
                    return Result.Failure("Car brand not found");
            }

            if (request.CarModelId.HasValue)
            {
                var carModel = await _unitOfWork.CarModels.GetByIdAsync(request.CarModelId.Value);
                if (carModel is null)
                    return Result.Failure("Car model not found");
            }

            specialization.Update(request.Name, request.Icon, request.CarBrandId, request.CarModelId);
            await _unitOfWork.Specializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Specialization updated successfully");
            return Result.Success("Specialization updated successfully");
        }
    }
}