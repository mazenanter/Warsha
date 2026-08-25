using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.Services.GetServiceById
{
    public class GetWorkshopServiceByIdQuery : IRequest<Result<WorkshopServiceDetailsResponseDto>>
    {
        public int WorkshopId { get; set; }
    }
}
