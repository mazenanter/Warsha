using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.CarCatalog.Queries.GetCarBrands
{
    public class GetCarBrandsQuery : IRequest<Result<List<CarBrandsResponseDto>>>
    {
    }
}
