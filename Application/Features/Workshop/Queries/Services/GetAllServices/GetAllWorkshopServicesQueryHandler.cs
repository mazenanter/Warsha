using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Features.Workshop.Queries.Services.GetAllServices
{
    public class GetAllWorkshopServicesQueryHandler : IRequestHandler<GetAllWorkshopServicesQuery, Result<PagedResult<WorkshopServiceResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllWorkshopServicesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<WorkshopServiceResponseDto>>> Handle(GetAllWorkshopServicesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<WorkshopService> query = _unitOfWork.WorkshopServices
    .GetAll()
    .Include(x => x.ServiceCategory);


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x =>
        x.NameEn.Contains(request.SearchTerm) ||
        x.NameAr.Contains(request.SearchTerm));
            }
            var totalRecords = await query.CountAsync(cancellationToken);
            var data = await query
                .OrderByDescending(je => je.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(je => new WorkshopServiceResponseDto
                {
                    Id = je.Id,
                  NameAr = je.NameAr,
                  NameEn = je.NameEn,
                  MaxPrice = je.MaxPrice,
                  MinPrice = je.MinPrice,
                  Category = je.ServiceCategory.Name,
                  Duration = je.DurationMin,
                  IsActive = je.IsVisible
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<WorkshopServiceResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<WorkshopServiceResponseDto>>.Success(pagedResult, "Services retrieved successfully");
        }
    }
}
