using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Workshop.Queries.Services.GetServiceById
{
    public class GetWorkshopServiceByIdQueryHandler : IRequestHandler<GetWorkshopServiceByIdQuery, Result<WorkshopServiceDetailsResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkshopServiceByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<WorkshopServiceDetailsResponseDto>> Handle(GetWorkshopServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var workshopService = await _unitOfWork.WorkshopServices.GetAll()
                .Include(x => x.ServiceCategory)
                .FirstOrDefaultAsync(x => x.Id == request.WorkshopId);

            if(workshopService is null)
            {
                return Result<WorkshopServiceDetailsResponseDto>.Failure("Service not found");
            }
            var response = new WorkshopServiceDetailsResponseDto
            {
                Category = workshopService.ServiceCategory.Name,
                DescriptionAr = workshopService.DescriptionAr,
                DescriptionEn = workshopService.DescriptionEn,Duration = workshopService.DurationMin,
                Id = workshopService.Id,
                IsActive = workshopService.IsVisible,
                MaxPrice = workshopService.MaxPrice,
                MinPrice = workshopService
                .MinPrice,
                NameAr = workshopService.NameAr,
                NameEn = workshopService.NameEn
            };
            return Result<WorkshopServiceDetailsResponseDto>.Success(response, "Service get successfully");
        }
    }
}
