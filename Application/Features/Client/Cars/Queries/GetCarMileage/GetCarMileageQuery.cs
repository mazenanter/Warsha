using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetCarMileage
{

    public record GetCarMileageQuery(int CarId)
    : IRequest<Result<MileageDto>>;

    public class GetCarMileageQueryHandler
        : IRequestHandler<GetCarMileageQuery, Result<MileageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetCarMileageQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<MileageDto>> Handle(
            GetCarMileageQuery request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var client = await _unitOfWork.Clients
                .FindAsync(x => x.UserId == userId);

            if (client is null)
                return Result<MileageDto>.Failure(
                    "Client not found");

            var car = await _unitOfWork.Cars
                .GetByIdAsync(request.CarId, ct);

            if (car is null)
                return Result<MileageDto>.Failure(
                    "Car not found");

            if (car.ClientId != client.Id)
                return Result<MileageDto>.Failure(
                    "You don't own this car");

            return Result<MileageDto>.Success(
                new MileageDto(
                    car.ActualOdometerKm,
                    car.GetEstimatedCurrentMileage(),
                    car.GpsAccumulatedKm,
                    car.GpsMileageLastUpdatedAt),"Car mileage retrieved successfully");
        }
    }
}
