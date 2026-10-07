using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetAll
{
    public class GetAllCarsQueryHandler : IRequestHandler<GetAllCarsQuery, Result<List<CarResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<CarResponseDto>>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _unitOfWork.Cars.GetAllCars();

            if (cars == null)
            {
                return Result<List<CarResponseDto>>.Failure("No cars found.");
            }

            var carDtos = cars.Select(car => new CarResponseDto
            {
                Id = car.Id,
                Brand = car.CarModel.CarBrand.Name,
                Model = car.CarModel.Name,
                Year = car.Year,
                ActualOdometerKm = car.ActualOdometerKm
            }).ToList();
            return Result<List<CarResponseDto>>.Success(carDtos,"Cars retrieved successfully.");
        }
    }
}
