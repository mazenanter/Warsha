using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommandHandler : IRequestHandler<AddSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddSpecializationCommandHandler> _logger;

        public AddSpecializationCommandHandler(IUnitOfWork unitOfWork, ILogger<AddSpecializationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(AddSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specializationExists = await _unitOfWork.Specializations.FindAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (specializationExists != null)
            {
                _logger.LogWarning("Specialization already exist ");
                return Result.Failure("Specialization already exist.");
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

            var newSpecialization = Domain.Entities.Specialization.Create(request.Name, request.Icon, request.CarBrandId, request.CarModelId);
            await _unitOfWork.Specializations.AddAsync(newSpecialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Specialization {Name} added successfully", request.Name);
            return Result.Success("Specialization added successfully.");
        }
    }
}