
using Application.Features.Client.Workshops.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Workshops.Queries.GetById
{
    public class GetWorkshopByIdQuery : IRequest<Result<WorkshopDetailsResponseDto>>
    {
        public int WorkshopId {get; set;}
        public int ReviewsPageNumber { get; set; } = 1;
        public int ReviewsPageSize { get; set; } = 10;
    }
}
