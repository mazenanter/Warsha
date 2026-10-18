using Application.Features.Admin.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries.GetClients
{
    public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<AdminClientListResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetClientsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdminClientListResponseDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Search is the only thing that narrows the tab counts
            var searched = _unitOfWork.Clients.GetAll().AsNoTracking();
            var search = request.Search?.Trim();
            if (!string.IsNullOrEmpty(search))
                searched = searched.Where(c => c.Name.Contains(search) || c.PhoneNumber.Contains(search));

            // Query 1: all four tab counts in a single SQL round trip
            var counts = await searched
                .GroupBy(_ => 1)
                .Select(g => new AdminClientCountsDto
                {
                    All = g.Count(),
                    Active = g.Count(c => c.IsActive),
                    Suspended = g.Count(c => !c.IsActive),
                    New = g.Count(c => c.CreatedAt >= monthStart)
                })
                .FirstOrDefaultAsync(cancellationToken) ?? new AdminClientCountsDto();

            // The selected tab decides both the extra filter and totalCount (no extra count query needed)
            var filtered = searched;
            int totalCount;
            switch (request.Status)
            {
                case ClientStatusFilter.Active:
                    filtered = searched.Where(c => c.IsActive);
                    totalCount = counts.Active;
                    break;
                case ClientStatusFilter.Suspended:
                    filtered = searched.Where(c => !c.IsActive);
                    totalCount = counts.Suspended;
                    break;
                case ClientStatusFilter.New:
                    filtered = searched.Where(c => c.CreatedAt >= monthStart);
                    totalCount = counts.New;
                    break;
                default:
                    totalCount = counts.All;
                    break;
            }

            // Query 2: one page of clients, newest first, with their cars count
            var rows = await filtered
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    c.CreatedAt,
                    CarsCount = c.Cars.Count()
                })
                .ToListAsync(cancellationToken);

            // Query 3: booking stats for only the clients on this page
            var stats = new Dictionary<int, (int Count, decimal Spending)>();
            if (rows.Count > 0)
            {
                var ids = rows.Select(r => r.Id).ToList();

                var grouped = await _unitOfWork.Bookings.GetAll()
                    .AsNoTracking()
                    .Where(b => ids.Contains(b.ClientId))
                    .GroupBy(b => b.ClientId)
                    .Select(g => new
                    {
                        ClientId = g.Key,
                        Count = g.Count(),
                        Spending = g.Where(b => b.BookingStatus == BookingStatus.Completed)
                                    .Sum(b => (decimal?)b.TotalAmount) ?? 0m
                    })
                    .ToListAsync(cancellationToken);

                stats = grouped.ToDictionary(x => x.ClientId, x => (x.Count, x.Spending));
            }

            var items = rows.Select(r =>
            {
                stats.TryGetValue(r.Id, out var s);
                return new AdminClientListItemDto
                {
                    Id = r.Id,
                    FullName = r.Name,
                    Phone = r.PhoneNumber,
                    Email = r.Email,
                    CarsCount = r.CarsCount,
                    BookingsCount = s.Count,
                    TotalSpending = s.Spending,
                    Status = r.IsActive ? "active" : "suspended",
                    CreatedAt = r.CreatedAt
                };
            }).ToList();

            var response = new AdminClientListResponseDto
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Counts = counts
            };

            return Result<AdminClientListResponseDto>.Success(response, "Clients retrieved successfully");
        }
    }
}