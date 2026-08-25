using Application.Features.Specialization.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Queries.GetAll
{
    public class GetAllSpecializationQuery : IRequest<Result<PagedResult<SpecializationResponseDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
