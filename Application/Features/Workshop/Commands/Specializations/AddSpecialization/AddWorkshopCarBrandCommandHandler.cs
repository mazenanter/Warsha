using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Specializations.AddSpecialization
{
    public class AddWorkshopCarBrandCommandHandler
        : IRequestHandler<AddWorkshopCarBrandCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddWorkshopCarBrandCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(
            AddWorkshopCarBrandCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workshopId = await _unitOfWork.Workshops.FindAsync(x => x.UserId == userId);

            if (workshopId is null)
            {
                return Result.Failure(
                    "Current user is not associated with a workshop.");
            }

            var workshop = await _unitOfWork.Workshops
                .GetByIdWithCarBrandsAsync(
                   workshopId.Id,
                    cancellationToken);

            if (workshop is null)
            {
                return Result.Failure("Workshop not found.");
            }

            var carBrand = await _unitOfWork.CarBrands
                .GetByIdAsync(request.CarBrandId, cancellationToken);

            if (carBrand is null)
            {
                return Result.Failure("Car brand not found.");
            }
            if (workshop.SupportedCarBrands.Any(
    x => x.CarBrandId == request.CarBrandId))
            {
                return Result.Failure(
                    "Car brand is already supported by this workshop.");
            }
            workshop.AddSupportedCarBrand(request.CarBrandId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(
               
                "Car brand added successfully.");
        }
    }
}
