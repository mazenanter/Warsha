using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Cars.Commands.StartTrip
{
    public class StartTripCommandHandler
    : IRequestHandler<StartTripCommand, Result<StartTripResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<StartTripCommandHandler> _logger;

        public StartTripCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            ILogger<StartTripCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<StartTripResult>> Handle(
            StartTripCommand command, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            var client = await _unitOfWork.Clients.FindAsync(x => x.UserId == userId);
            if (client is null)
                return Result<StartTripResult>.Failure("Client not found");
            var car = await _unitOfWork.Cars.FindAsync(
    x => x.Id == command.CarId && x.ClientId == client.Id);
            if (car is null)
                return Result<StartTripResult>.Failure("Car not found");

            if (car.ClientId != client.Id)
                return Result<StartTripResult>.Failure("You don't own this car");

            var activeTrip = await _unitOfWork.Trips
                .GetActiveByCarIdAsync(command.CarId, ct);

            if (activeTrip != null)
                return Result<StartTripResult>.Failure(
                    $"Car already has an active trip (ID: {activeTrip.Id}). " +
                    "Complete or cancel it before starting a new one.");

            var trip = Trip.StartGps(command.CarId, command.Latitude, command.Longitude);
            await _unitOfWork.Trips.AddAsync(trip, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Trip {TripId} started for car {CarId} by client {ClientId}",
                trip.Id, command.CarId, client.Id);

            return Result<StartTripResult>.Success(new StartTripResult(
                trip.Id,
                trip.Status.ToString(),
                trip.StartedAt,
                trip.StartLatitude,
                trip.StartLongitude),
                "Trip started successfully");
        }
    }
}
