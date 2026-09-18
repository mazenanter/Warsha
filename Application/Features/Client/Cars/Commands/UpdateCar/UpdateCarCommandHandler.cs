using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Client.Cars.Commands.UpdateCar
{
    public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCarCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if(car == null)
            {
                return Result.Failure("Car not found.");
            }

            var clientId = _currentUserService.UserId;
            var client = await _unitOfWork.Clients.FindAsync(x=>x.UserId == clientId);
            if (client.Cars.Any(x =>
        x.CarModelId == request.CarModel &&
          x.Year == request.Year))
            {
                return Result.Failure(
                    "You already have a car with the same model and year."
                );
            }
            car.UpdateCar(request.CarModel, request.Year,null);
            await _unitOfWork.Cars.UpdateAsync(car);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Car updated successfully.");

        }
    }
}
