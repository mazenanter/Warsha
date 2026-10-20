using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.GetDashboardStats
{
    public class GetDashboardStatsQuery : IRequest<Result<DashboardStatsResponseDto>>
    {
    }
}