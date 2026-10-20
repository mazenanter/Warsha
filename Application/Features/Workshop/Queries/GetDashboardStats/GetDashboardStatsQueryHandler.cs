using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Workshop.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsResponseDto>>
    {
        // Statuses that count as a "real" booking on the dashboard
        private static readonly BookingStatus[] CountedStatuses =
        {
            BookingStatus.PendingApproval,
            BookingStatus.PendingCapture,
            BookingStatus.Confirmed,
            BookingStatus.Completed
        };

        private static readonly TimeZoneInfo LocalZone = ResolveLocalZone();

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<DashboardStatsResponseDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var workshopId = _currentUserService.WorkshopId;
            if (workshopId is null)
                return Result<DashboardStatsResponseDto>.Failure("Workshop not found in token");

            // Day boundaries in the workshop's local time, converted to UTC for the queries
            var nowUtc = DateTime.UtcNow;
            var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, LocalZone).Date;
            var yesterdayStartUtc = LocalMidnightToUtc(todayLocal.AddDays(-1));
            var todayStartUtc = LocalMidnightToUtc(todayLocal);
            var tomorrowStartUtc = LocalMidnightToUtc(todayLocal.AddDays(1));

            var bookings = _unitOfWork.Bookings.GetAll()
                .AsNoTracking()
                .Where(b => b.WorkshopId == workshopId.Value);

            // 1) Bookings scheduled yesterday / today (one grouped query)
            var scheduled = await bookings
                .Where(b => CountedStatuses.Contains(b.BookingStatus)
                         && b.ScheduledAt >= yesterdayStartUtc
                         && b.ScheduledAt < tomorrowStartUtc)
                .GroupBy(b => b.ScheduledAt >= todayStartUtc)
                .Select(g => new { IsToday = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var todaysBookings = scheduled.FirstOrDefault(x => x.IsToday)?.Count ?? 0;
            var yesterdaysBookings = scheduled.FirstOrDefault(x => !x.IsToday)?.Count ?? 0;

            // 2) Cars currently at the workshop
            var carsInService = await bookings.CountAsync(b =>
                b.BookingStatus == BookingStatus.Confirmed &&
                (b.JobStatus == JobStatus.Diagnosing ||
                 b.JobStatus == JobStatus.InProgress ||
                 b.JobStatus == JobStatus.Ready),
                cancellationToken);

            // 3) Requests the workshop can still accept or reject
            var awaitingApproval = await bookings.CountAsync(b =>
                b.BookingStatus == BookingStatus.PendingApproval &&
                b.ResponseDeadline > nowUtc,
                cancellationToken);

            // 4) Revenue from jobs completed yesterday / today (one grouped query)
            var revenueRows = await bookings
                .Where(b => b.BookingStatus == BookingStatus.Completed
                         && b.CompletedAt >= yesterdayStartUtc
                         && b.CompletedAt < tomorrowStartUtc)
                .GroupBy(b => b.CompletedAt >= todayStartUtc)
                .Select(g => new { IsToday = g.Key, Amount = g.Sum(b => b.ServicesAmount) })
                .ToListAsync(cancellationToken);

            var revenueToday = revenueRows.FirstOrDefault(x => x.IsToday)?.Amount ?? 0m;
            var revenueYesterday = revenueRows.FirstOrDefault(x => !x.IsToday)?.Amount ?? 0m;

            decimal? changePercent = revenueYesterday > 0
                ? Math.Round((revenueToday - revenueYesterday) / revenueYesterday * 100m, 1)
                : null;

            // 5) Rating and review count
            var reviewStats = await _unitOfWork.Reviews.GetAll()
                .AsNoTracking()
                .Where(r => r.WorkshopId == workshopId.Value)
                .GroupBy(_ => 1)
                .Select(g => new { Count = g.Count(), Average = g.Average(r => r.Rating) })
                .FirstOrDefaultAsync(cancellationToken);

            var response = new DashboardStatsResponseDto
            {
                TodaysBookings = todaysBookings,
                YesterdaysBookings = yesterdaysBookings,
                CarsInService = carsInService,
                AwaitingApproval = awaitingApproval,
                RevenueToday = new RevenueStatDto
                {
                    Amount = revenueToday,
                    ChangePercent = changePercent
                },
                AverageRating = reviewStats is null ? 0 : Math.Round(reviewStats.Average, 2),
                ReviewsCount = reviewStats?.Count ?? 0
            };

            return Result<DashboardStatsResponseDto>.Success(response, "Dashboard stats retrieved successfully");
        }

        private static TimeZoneInfo ResolveLocalZone()
        {
            // IANA id works on Linux and modern Windows; the second is the classic Windows id
            foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return TimeZoneInfo.Utc;
        }

        private static DateTime LocalMidnightToUtc(DateTime localDate)
        {
            var unspecified = DateTime.SpecifyKind(localDate, DateTimeKind.Unspecified);

            // Egypt's DST starts at midnight, so 00:00 doesn't exist on that day; use 01:00 instead of throwing
            if (LocalZone.IsInvalidTime(unspecified))
                unspecified = unspecified.AddHours(1);

            return TimeZoneInfo.ConvertTimeToUtc(unspecified, LocalZone);
        }
    }
}