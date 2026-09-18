using Application.Features.Client.Cars.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Client.Cars.Queries.GetById
{
    public class GetCarByIdQuery : IRequest<Result<CarResponseDto>>
    {
        public int CarId { get; set; }
    }
}
