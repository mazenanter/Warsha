using Application.Features.Workshop.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Queries.GetDashboardRevenue
{
    public class GetDashboardRevenueQuery : IRequest<Result<RevenueChartResponseDto>>
    {
        // Optional. Any date inside the wanted week (normally that week's Monday). Defaults to this week.
        public DateOnly? WeekStart { get; set; }
    }
}