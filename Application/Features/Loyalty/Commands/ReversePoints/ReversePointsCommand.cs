using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.ReversePoints
{
    public record ReversePointsCommand(
      int ClientId,
      int BookingId,
      string Reason
  ) : IRequest<Result>;
}
