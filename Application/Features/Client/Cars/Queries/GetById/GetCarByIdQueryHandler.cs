using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetById
{
    public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Result<CarResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CarResponseDto>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
           var car = await _unitOfWork.Cars.GetCarById(request.CarId);
            if (car == null)
            {
                return Result<CarResponseDto>.Failure("Car not found.");
            }
            var carResponseDto = new CarResponseDto
            {
                Id = car.Id,
                Brand = car.CarModel.CarBrand.Name,
                Model = car.CarModel.Name,
                Year = car.Year,
                ActualOdometerKm = car.ActualOdometerKm
            };
            return Result<CarResponseDto>.Success(carResponseDto, "Car retrieved successfully.");
        }
    }
}
