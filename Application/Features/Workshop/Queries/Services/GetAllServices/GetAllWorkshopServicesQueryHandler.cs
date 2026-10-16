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
        private readonly ICurrentUserService _currentUserService;

        public GetAllWorkshopServicesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<WorkshopServiceResponseDto>>> Handle(GetAllWorkshopServicesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workshop = await _unitOfWork.Workshops.FindAsync(x => x.UserId == userId);
            if (workshop is null) return Result<PagedResult<WorkshopServiceResponseDto>>.Failure("Workshop not found");

            IQueryable<WorkshopService> query = _unitOfWork.WorkshopServices
    .GetAll().Where(x => x.WorkshopId == workshop.Id)
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
                  DescriptionAr = je.DescriptionAr,
                    DescriptionEn = je.DescriptionEn,
                    CategoryEn = je.ServiceCategory.NameEn,
                  CategoryAr = je.ServiceCategory.NameAr,
                  Duration = je.DurationMin,
                  IsActive = je.IsVisible
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<WorkshopServiceResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<WorkshopServiceResponseDto>>.Success(pagedResult, "Services retrieved successfully");
        }
    }
}
