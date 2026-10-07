using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.CompleteTrip
{
    public record CompleteTripCommand(
     int TripId,
     decimal DistanceKm,
     double Latitude,
     double Longitude
 ) : IRequest<Result<CompleteTripResult>>;
}
