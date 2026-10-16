using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetAll
{
    public class GetAllCarsQueryHandler : IRequestHandler<GetAllCarsQuery, Result<List<CarResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetAllCarsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<CarResponseDto>>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
           var userId = _currentUserService.UserId;
            var client = await _unitOfWork.Clients.FindAsync(x => x.UserId == userId);

            if (client is null)
                return Result<List<CarResponseDto>>.Failure(
                    "Client profile not found.");

            var cars = await _unitOfWork.Cars.GetByClientIdAsync(
                client.Id,
                cancellationToken);

            var carDtos = cars.Select(car => new CarResponseDto
            {
                Id = car.Id,
                Brand = car.CarModel.CarBrand.Name,
                Model = car.CarModel.Name,
                Year = car.Year,
                ActualOdometerKm = car.ActualOdometerKm
            }).ToList();

            return Result<List<CarResponseDto>>.Success(
                carDtos,
                "Cars retrieved successfully.");
        }
    }
}
