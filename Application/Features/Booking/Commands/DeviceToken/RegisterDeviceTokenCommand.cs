using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.DeviceToken
{
    public record RegisterDeviceTokenCommand(
     string DeviceId,
     string Token,
     string Platform) : IRequest<Result>;

}
