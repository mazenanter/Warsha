using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Client.Cars.Commands.CompleteTrip
{
    public class CompleteTripCommandHandler
         : IRequestHandler<CompleteTripCommand, Result<CompleteTripResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IPublisher _publisher;
        private readonly ILogger<CompleteTripCommandHandler> _logger;

        public CompleteTripCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IPublisher publisher,
            ILogger<CompleteTripCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<Result<CompleteTripResult>> Handle(
            CompleteTripCommand command,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var client = await _unitOfWork.Clients
                .FindAsync(x => x.UserId == userId);

            if (client is null)
                return Result<CompleteTripResult>.Failure(
                    "Client not found");

            var trip = await _unitOfWork.Trips
                .GetByIdWithCarAsync(command.TripId, ct);

            if (trip is null)
                return Result<CompleteTripResult>.Failure(
                    "Trip not found");

            if (trip.Car.ClientId != client.Id)
                return Result<CompleteTripResult>.Failure(
                    "You don't own this trip's car");

            if (trip.Status == TripStatus.Completed)
            {
                _logger.LogInformation(
                    "Duplicate complete request for trip {TripId}",
                    trip.Id);

                return Result<CompleteTripResult>.Success(
                    new CompleteTripResult(
                        trip.Id,
                        trip.Status.ToString(),
                        trip.StartedAt,
                        trip.EndedAt!.Value,
                        trip.DistanceKm!.Value,
                        trip.Car.GetEstimatedCurrentMileage()),
                    "Trip already completed");
            }

            if (trip.Status != TripStatus.Active)
                return Result<CompleteTripResult>.Failure(
                    $"Cannot complete a trip with status '{trip.Status}'");
            trip.Complete(
                command.DistanceKm,
                command.Latitude,
                command.Longitude
              );

            var car = trip.Car;

            car.AddGpsMileage(command.DistanceKm);

            await _unitOfWork.SaveChangesAsync(ct);

            var estimatedMileage =
                car.GetEstimatedCurrentMileage();

            _logger.LogInformation(
                "Trip {TripId} completed: {DistanceKm} km. " +
                "Car {CarId} estimated mileage now {Mileage} km",
                trip.Id,
                command.DistanceKm,
                car.Id,
                estimatedMileage);

            await _publisher.Publish(
                new TripCompletedEvent(
                    trip.Id,
                    car.Id,
                    client.Id,
                    command.DistanceKm),
                ct);

            return Result<CompleteTripResult>.Success(
                new CompleteTripResult(
                    trip.Id,
                    trip.Status.ToString(),
                    trip.StartedAt,
                    trip.EndedAt!.Value,
                    trip.DistanceKm!.Value,
                    estimatedMileage),
                "Trip completed successfully");
        }
    }
}
