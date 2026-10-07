using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.CorrectOdometer
{
    public class CorrectOdometerCommandHandler
     : IRequestHandler<CorrectOdometerCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CorrectOdometerCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(CorrectOdometerCommand command, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;

            var car = await _unitOfWork.Cars.GetByIdWithCorrectionsAsync(command.CarId, ct);
            if (car is null)
                return Result.Failure("Car not found");

            if (car.ClientId != clientId)
                return Result.Failure("You don't own this car");

            car.CorrectOdometer(command.OdometerKm, command.Note);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(
                $"Odometer updated to {command.OdometerKm:N0} km successfully");
        }
    }
}
