using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.ServiceCategory.GetAll
{
    public record GetAllServiceCategoriesQuery
      : IRequest<Result<List<ServiceCategoryDto>>>;
}
