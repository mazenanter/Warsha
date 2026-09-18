using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.CarCatalog.Queries.GetCarModels
{
    public class GetCarModelsQuery : IRequest<Result<List<CarModelResponseDto>>>
    {
        public int CarBrandId { get; set; }
    }
}
