
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

            var workshop =  await _unitOfWork.Workshops.GetAll()
            .Include(w => w.Services)
            .ThenInclude(w => w.ServiceCategory)
            .Where(w => w.Id == request.WorkshopId && w.IsVerified)
            .FirstOrDefaultAsync(cancellationToken);

            if(workshop == null)
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
            .Select( r => new ReviewResponseDto
            {
                Id = r.Id,
                ClientName = r.Client.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToListAsync(cancellationToken);

            var result = new WorkshopDetailsResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Address = workshop.Address,
                Phone = workshop.Phone,
                Lat = workshop.Lat,
                Lng = workshop.Lng,
                RatingAvg = workshop.RatingAvg,
                GoogleMapsLink = workshop.GoogleMapsLink,
                OpeningTime = workshop.OpeningTime,
                ClosingTime = workshop.ClosingTime,
                StartBusyTime = workshop.BusyFrom,
                BusyUntil = workshop.BusyUntil,
                BusyStatus = workshop.IsBusy,
                Services = workshop.Services
                .Where(s => s.IsVisible)
                .Select(s => new WorkshopServiceResponseDto
                {
                    Id = s.Id,
                    NameEn = s.NameEn,
                    NameAr = s.NameAr,
                    CategoryEn = s.ServiceCategory.NameEn,
                    CategoryAr = s.ServiceCategory.NameAr,
                    Duration = s.DurationMin,
                    MinPrice = s.MinPrice,
                    MaxPrice = s.MaxPrice,
                    IsActive = s.IsVisible
                }).ToList(),
                Reviews = PagedResult<ReviewResponseDto>
                .Create(reviews, totalReviews, request.ReviewsPageNumber, request.ReviewsPageSize)
            };

            return Result<WorkshopDetailsResponseDto>.Success(result, "Workshop details retrieved successfully");

        }

    }
}
