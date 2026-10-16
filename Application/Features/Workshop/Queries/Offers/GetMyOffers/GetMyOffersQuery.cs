using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.Offers.GetMyOffers
{
    public record GetMyOffersQuery : IRequest<Result<List<WorkshopOfferDto>>>;
}
