using Application.Features.Client.Workshops.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.SavedWorkshops.Queries.GetSavedWorkshops
{
    public class GetSavedWorkshopsQuery : IRequest<Result<PagedResult<WorkshopResponseDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}