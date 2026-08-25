using Application.Features.Specialization.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Specialization.Queries.GetById
{
    public class GetSpecializationByIdQuery : IRequest<Result<SpecializationResponseDto>>
    {
        public int Id { get; set; }
    }
}
