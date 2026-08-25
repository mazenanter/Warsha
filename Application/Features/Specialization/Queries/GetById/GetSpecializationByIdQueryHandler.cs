using Application.Features.Specialization.DTOs;
using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialization.Queries.GetById
{
    public class GetSpecializationByIdQueryHandler : IRequestHandler<GetSpecializationByIdQuery, Result<SpecializationResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSpecializationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SpecializationResponseDto>> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.FindAsync(x=>x.Id == request.Id && x.IsActive);

            if (specialization is null)
            {
                return Result<SpecializationResponseDto>.Failure("Specialization not found");
            }
            var response = new SpecializationResponseDto
            {
               
                Id = specialization.Id,
               Name = specialization.Name,
               Icon = specialization.Icon
            };
            return Result<SpecializationResponseDto>.Success(response, "Specialization get successfully");
        }
    }
}
