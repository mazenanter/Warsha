using Application.Features.Workshop.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Workshop.Queries.Offers.GetMyOffers
{
    public class GetMyOffersQueryHandler
     : IRequestHandler<GetMyOffersQuery, Result<List<WorkshopOfferDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyOffersQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<List<WorkshopOfferDto>>> Handle(
            GetMyOffersQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            var workshop = await _unitOfWork.Workshops
                .GetByUserIdWithOffersAsync(userId, cancellationToken);

            if (workshop is null)
                return Result<List<WorkshopOfferDto>>.Failure(
                    "Workshop not found");
            var now = DateTime.UtcNow;

            var offers = workshop.Offers
                .Where(o =>
                    o.IsActive &&
                    o.EndAt >= now)
                .Select(offer => new WorkshopOfferDto
                {
                    Id = offer.Id,
                    WorkshopServiceId = offer.WorkshopServiceId,
                    ServiceNameEn = offer.WorkshopService?.NameEn,
                    ServiceNameAr = offer.WorkshopService?.NameAr,
                    DiscountPercentage = offer.DiscountPercentage,
                    StartAt = offer.StartAt,
                    EndAt = offer.EndAt,
                    IsActive = offer.IsActive
                })
                .OrderBy(offer => offer.StartAt)
                .ToList();

            return Result<List<WorkshopOfferDto>>.Success(offers,"Offers retrieved successfully");
        }
    }
}
