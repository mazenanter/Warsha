using Application.Features.Client.Workshops.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Workshops.Queries.GetRecommendedWorkshops
{
    public class GetRecommendedWorkshopsQueryHandler : IRequestHandler<GetRecommendedWorkshopsQuery, Result<PagedResult<WorkshopResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public GetRecommendedWorkshopsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<WorkshopResponseDto>>> Handle(GetRecommendedWorkshopsQuery request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if(clientId is null)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure("User Id Doesn't exist");
            }
            var cars = await _unitOfWork.Cars.GetAll()
            .Include(c => c.CarModel)
            .Where(c => c.ClientId == clientId.Value)
            .ToListAsync(cancellationToken);

            if (cars.Count == 0)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure("Add Car First to Get recommendation");
            }

            var carBrandIds = cars.Select(c => c.CarModel.CarBrandId).Distinct().ToList();
            var carModelIds = cars.Select(c => c.CarModelId).Distinct().ToList();

            var query = _unitOfWork.Workshops.GetAll()
                .Where(w => w.IsVerified && w.Specializations.Any(ws =>
                    (ws.Specialization.CarBrandId != null && carBrandIds.Contains(ws.Specialization.CarBrandId.Value)) ||
                    (ws.Specialization.CarModelId != null && carModelIds.Contains(ws.Specialization.CarModelId.Value))))
                .OrderByDescending(w => w.RatingAvg);


            var totalRecords = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(w => new WorkshopResponseDto
                {
                    UserId = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    Phone = w.Phone,
                    Lat = w.Lat,
                    Lng = w.Lng,
                    RatingAvg = w.RatingAvg,
                    GoogleMapsLink = w.GoogleMapsLink,
                    OpeningTime = w.OpeningTime,
                    ClosingTime = w.ClosingTime,
                    BusyUntil = w.BusyUntil,
                    StartBusyTime = w.BusyFrom,
                    BusyStatus = w.IsBusy,
                    DistanceKM = null
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<WorkshopResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<WorkshopResponseDto>>.Success(pagedResult, "Recommended workshops retrieved successfully");

        }
    }
}