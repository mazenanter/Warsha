using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.ManualAdjustment
{
    public record ManualAdjustmentCommand(
     int ClientId,
     int Amount,
     string Reason
 ) : IRequest<Result>;
}
