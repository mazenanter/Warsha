using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Offers.DeleteOffer
{
    public record DeleteOfferCommand(int OfferId)
    : IRequest<Result>;
}
