using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Client.Cars.Commands.AddCar
{
    public class AddCarCommandHandler : IRequestHandler<AddCarCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public AddCarCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AddCarCommand request, CancellationToken cancellationToken)
        {
           
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
            client.AddCar(request.CarModel, request.Year, null, null);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Car added successfully.");
        }


    }
}
