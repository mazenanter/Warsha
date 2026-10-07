using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.StartTrip
{
    public record StartTripCommand(
     int CarId,
     double Latitude,
     double Longitude
 ) : IRequest<Result<StartTripResult>>;
}
