using Application.Features.Client.Workshops.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Workshops.Queries.GetRecommendedWorkshops
{
    public class GetRecommendedWorkshopsQuery : IRequest<Result<PagedResult<WorkshopResponseDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}