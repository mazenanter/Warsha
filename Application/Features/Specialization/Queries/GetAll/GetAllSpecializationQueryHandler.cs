using Application.Features.Specialization.DTOs;
using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialization.Queries.GetAll
{
    public class GetAllSpecializationQueryHandler : IRequestHandler<GetAllSpecializationQuery, Result<PagedResult<SpecializationResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSpecializationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<SpecializationResponseDto>>> Handle(GetAllSpecializationQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Domain.Entities.Specialization> query = _unitOfWork.Specializations
     .GetAll()
     .Where(x=>x.IsActive);


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x =>
        x.Name.Contains(request.SearchTerm));
            }
            var totalRecords = await query.CountAsync(cancellationToken);
            var data = await query
                .OrderByDescending(je => je.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(je => new SpecializationResponseDto
                {
                    Id = je.Id,
                   Name = je.Name,
                   Icon = je.Icon
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<SpecializationResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<SpecializationResponseDto>>.Success(pagedResult, "Specializations retrieved successfully");
        }
    }
}
