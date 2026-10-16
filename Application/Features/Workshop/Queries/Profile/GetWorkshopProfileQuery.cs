using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.Profile
{
    public class GetWorkshopProfileQuery : IRequest<Result<WorkshopProfileResponseDto>>
    {
    }
}
