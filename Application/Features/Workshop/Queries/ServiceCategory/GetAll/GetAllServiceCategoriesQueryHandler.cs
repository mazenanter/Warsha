using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.ServiceCategory.GetAll
{
    public class GetAllServiceCategoriesQueryHandler
      : IRequestHandler<
          GetAllServiceCategoriesQuery,
          Result<List<ServiceCategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllServiceCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ServiceCategoryDto>>> Handle(
            GetAllServiceCategoriesQuery request,
            CancellationToken ct)
        {
            var categories = _unitOfWork.ServiceCategories
                .GetAll();

            var result = categories
                .Select(x => new ServiceCategoryDto(
                    x.Id,
                    x.NameAr,
                    x.NameEn,
                    x.Icon))
                .ToList();

            return Result<List<ServiceCategoryDto>>.Success(
                result,
                "Service categories retrieved successfully");
        }
    }
}
