using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.DeleteCar
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCarCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var client = await _unitOfWork.Clients
                .FindAsync(x => x.UserId == userId);

            if (client is null)
                return Result.Failure("Client profile not found.");

            var car = await _unitOfWork.Cars
                .GetByIdAsync(request.CarId, cancellationToken);

            if (car is null || car.ClientId != client.Id || car.IsDeleted)
                return Result.Failure("Car not found.");

            car.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Car deleted successfully.");
        }
    }
}
