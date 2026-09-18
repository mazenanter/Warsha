using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Offers.CreateOffer
{
    public record CreateOfferCommand
    (int? serviceId, decimal discountPercentage,DateTime startAt, DateTime endAt) : IRequest<Result>;
}
