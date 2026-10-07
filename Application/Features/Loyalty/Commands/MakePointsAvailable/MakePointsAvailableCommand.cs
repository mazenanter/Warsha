using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.MakePointsAvailable
{
    public record MakePointsAvailableCommand(int BookingId) : IRequest<Result>;
}
