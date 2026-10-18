using Application.Features.Client.Workshops.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Workshops.Queries.GetById
{
    public class GetWorkshopByIdQueryHandler : IRequestHandler<GetWorkshopByIdQuery, Result<WorkshopDetailsResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetWorkshopByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<WorkshopDetailsResponseDto>> Handle(GetWorkshopByIdQuery request, CancellationToken cancellationToken)
        {
            var workshop = await _unitOfWork.Workshops.GetAll()
                .Include(w => w.Services)
                .ThenInclude(w => w.ServiceCategory)
                .Where(w => w.Id == request.WorkshopId && w.IsVerified)
                .FirstOrDefaultAsync(cancellationToken);

            if (workshop == null)
            {
                return Result<WorkshopDetailsResponseDto>.Failure("failed to retrieve workshop");
            }

            var reviewQuery = _unitOfWork.Reviews.GetAll()
                .Include(r => r.Client)
                .Where(r => r.WorkshopId == request.WorkshopId)
                .OrderByDescending(r => r.CreatedAt);

            var totalReviews = await reviewQuery.CountAsync(cancellationToken);

            var reviews = await reviewQuery
                .Skip((request.ReviewsPageNumber - 1) * request.ReviewsPageSize)
                .Take(request.ReviewsPageSize)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    ClientName = r.Client.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToListAsync(cancellationToken);

            // ---- Offers: only currently valid ones (workshop-wide + service-specific) ----
            var now = DateTime.UtcNow;

            var activeOffers = await _unitOfWork.Workshops.GetAll()
                .AsNoTracking()
                .Where(w => w.Id == workshop.Id)
                .SelectMany(w => w.Offers)
                .Where(o => o.IsActive && o.DiscountPercentage > 0 && o.StartAt <= now && o.EndAt >= now)
                .Select(o => new { o.Id, o.WorkshopServiceId, o.DiscountPercentage, o.StartAt, o.EndAt })
                .ToListAsync(cancellationToken);

            var visibleServices = workshop.Services.Where(s => s.IsVisible).ToList();
            var serviceById = visibleServices.ToDictionary(s => s.Id);

            // Biggest workshop-wide discount: applies to every service
            var workshopWideDiscount = activeOffers
                .Where(o => o.WorkshopServiceId is null)
                .Select(o => Math.Min(o.DiscountPercentage, 100m))
                .DefaultIfEmpty(0m)
                .Max();

            // Biggest discount per specific service
            var serviceDiscounts = activeOffers
                .Where(o => o.WorkshopServiceId is not null)
                .GroupBy(o => o.WorkshopServiceId!.Value)
                .ToDictionary(g => g.Key, g => g.Max(o => Math.Min(o.DiscountPercentage, 100m)));

            // Offers list: skip offers whose service is hidden or missing
            var offers = activeOffers
                .Where(o => o.WorkshopServiceId is null || serviceById.ContainsKey(o.WorkshopServiceId.Value))
                .OrderByDescending(o => o.DiscountPercentage)
                .Select(o =>
                {
                    string? serviceNameEn = null;
                    string? serviceNameAr = null;
                    if (o.WorkshopServiceId is int serviceId && serviceById.TryGetValue(serviceId, out var service))
                    {
                        serviceNameEn = service.NameEn;
                        serviceNameAr = service.NameAr;
                    }

                    return new WorkshopOfferResponseDto
                    {
                        Id = o.Id,
                        DiscountPercentage = Math.Min(o.DiscountPercentage, 100m),
                        StartAt = o.StartAt,
                        EndAt = o.EndAt,
                        AppliesToAllServices = o.WorkshopServiceId is null,
                        ServiceId = o.WorkshopServiceId,
                        ServiceNameEn = serviceNameEn,
                        ServiceNameAr = serviceNameAr
                    };
                })
                .ToList();

            var result = new WorkshopDetailsResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                AddressEn = workshop.AddressEn,
                AddressAr = workshop.AddressAr,
                DescriptionEn = workshop.DescriptionEn ?? string.Empty,
                DescriptionAr = workshop.DescriptionAr ?? string.Empty,
                ImageUrl = workshop.ImageUrl,
                Phone = workshop.Phone,
                Lat = workshop.Lat,
                Lng = workshop.Lng,
                RatingAvg = workshop.RatingAvg,
                OpeningTime = workshop.OpeningTime,
                ClosingTime = workshop.ClosingTime,
                StartBusyTime = workshop.BusyFrom,
                BusyUntil = workshop.BusyUntil,
                BusyStatus = workshop.IsBusy,
                MaxDiscountPercentage = offers.Max(o => (decimal?)o.DiscountPercentage),
                Offers = offers,
                Services = visibleServices
                    .Select(s =>
                    {
                        serviceDiscounts.TryGetValue(s.Id, out var specificDiscount);
                        var bestDiscount = Math.Max(workshopWideDiscount, specificDiscount);
                        var hasDiscount = bestDiscount > 0;

                        return new WorkshopServiceResponseDto
                        {
                            Id = s.Id,
                            NameEn = s.NameEn,
                            NameAr = s.NameAr,
                            CategoryEn = s.ServiceCategory.NameEn,
                            CategoryAr = s.ServiceCategory.NameAr,
                            Duration = s.DurationMin,
                            MinPrice = s.MinPrice,
                            MaxPrice = s.MaxPrice,
                            IsActive = s.IsVisible,
                            DiscountPercentage = hasDiscount ? bestDiscount : null,
                            DiscountedMinPrice = hasDiscount ? ApplyDiscount(s.MinPrice, bestDiscount) : null,
                            DiscountedMaxPrice = hasDiscount ? ApplyDiscount(s.MaxPrice, bestDiscount) : null
                        };
                    }).ToList(),
                Reviews = PagedResult<ReviewResponseDto>
                    .Create(reviews, totalReviews, request.ReviewsPageNumber, request.ReviewsPageSize)
            };

            return Result<WorkshopDetailsResponseDto>.Success(result, "Workshop details retrieved successfully");
        }

        private static decimal ApplyDiscount(decimal price, decimal discountPercentage)
            => Math.Round(price * (100m - discountPercentage) / 100m, 2, MidpointRounding.AwayFromZero);
    }
}