using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Commands.CorrectOdometer
{
    public record CorrectOdometerCommand(
        int CarId,
        int OdometerKm,
        string? Note
    ) : IRequest<Result>;
}
