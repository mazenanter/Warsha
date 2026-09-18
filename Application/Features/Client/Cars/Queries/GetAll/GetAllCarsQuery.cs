using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetAll
{
    public class GetAllCarsQuery : IRequest<Result<List<CarResponseDto>>>
    {
    }
}
