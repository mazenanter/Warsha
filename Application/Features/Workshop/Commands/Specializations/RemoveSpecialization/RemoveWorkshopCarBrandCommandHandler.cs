using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.Specializations.RemoveSpecialization
{
    public class RemoveWorkshopCarBrandCommandHandler
         : IRequestHandler<RemoveWorkshopCarBrandCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveWorkshopCarBrandCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(
            RemoveWorkshopCarBrandCommand request,
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

            var supportedBrand = workshop.SupportedCarBrands
                .FirstOrDefault(x => x.CarBrandId == request.CarBrandId);

            if (supportedBrand is null)
            {
                return Result.Failure(
                    "This car brand is not supported by the workshop.");
            }

            workshop.RemoveSupportedCarBrand(request.CarBrandId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(
             
                "Car brand removed successfully.");
        }
    }
}
