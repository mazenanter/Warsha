using Application.Common.Helpers;
using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Workshop.Queries.GetDashboardRevenue
{
    public class GetDashboardRevenueQueryHandler : IRequestHandler<GetDashboardRevenueQuery, Result<RevenueChartResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardRevenueQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<RevenueChartResponseDto>> Handle(GetDashboardRevenueQuery request, CancellationToken cancellationToken)
        {
            var workshopId = _currentUserService.WorkshopId;
            if (workshopId is null)
                return Result<RevenueChartResponseDto>.Failure("Workshop not found in token");

            // Monday of the requested week (this week when not provided)
            var anchor = request.WeekStart ?? EgyptTime.TodayLocal();
            var monday = anchor.AddDays(-(((int)anchor.DayOfWeek + 6) % 7));

            var rangeStartUtc = EgyptTime.StartOfDayUtc(monday);
            var rangeEndUtc = EgyptTime.StartOfDayUtc(monday.AddDays(7));

            // Only two columns of one week's completed bookings are loaded
            var rows = await _unitOfWork.Bookings.GetAll()
                .AsNoTracking()
                .Where(b => b.WorkshopId == workshopId.Value
                         && b.BookingStatus == BookingStatus.Completed
                         && b.CompletedAt >= rangeStartUtc
                         && b.CompletedAt < rangeEndUtc)
                .Select(b => new { b.CompletedAt, b.ServicesAmount })
                .ToListAsync(cancellationToken);

            var amountByDay = rows
                .GroupBy(r => EgyptTime.ToLocalDate(r.CompletedAt!.Value))
                .ToDictionary(g => g.Key, g => g.Sum(r => r.ServicesAmount));

            // Always 7 days, Monday to Sunday, zero where there is no revenue
            var days = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    var date = monday.AddDays(i);
                    return new RevenueDayDto
                    {
                        Date = date,
                        Amount = amountByDay.GetValueOrDefault(date)
                    };
                })
                .ToList();

            var response = new RevenueChartResponseDto
            {
                Days = days,
                Total = days.Sum(d => d.Amount)
            };

            return Result<RevenueChartResponseDto>.Success(response, "Revenue retrieved successfully");
        }
    }
}