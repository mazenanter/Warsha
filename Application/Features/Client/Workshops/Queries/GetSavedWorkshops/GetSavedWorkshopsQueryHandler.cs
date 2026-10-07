using Application.Features.Client.SavedWorkshops.Queries.GetSavedWorkshops;
using Application.Features.Client.Workshops.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Workshops.Queries.GetSavedWorkshops
{
    public class GetSavedWorkshopsQueryHandler : IRequestHandler<GetSavedWorkshopsQuery, Result<PagedResult<WorkshopResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSavedWorkshopsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<WorkshopResponseDto>>> Handle(GetSavedWorkshopsQuery request, CancellationToken cancellationToken)
        {
            var clientId = _currentUserService.ClientId;
            if (clientId is null)
                return Result<PagedResult<WorkshopResponseDto>>.Failure("Client not found in token");

            var query = _unitOfWork.SavedWorkshops.GetAll()
                .Include(x => x.Workshop)
                .Where(x => x.ClientId == clientId.Value)
                .OrderByDescending(x => x.CreatedAt);

            var totalRecords = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new WorkshopResponseDto
                {
                    UserId = x.Workshop.Id,
                    Name = x.Workshop.Name,
                    Address = x.Workshop.Address,
                    Phone = x.Workshop.Phone,
                    Lat = x.Workshop.Lat,
                    Lng = x.Workshop.Lng,
                    RatingAvg = x.Workshop.RatingAvg,
                    GoogleMapsLink = x.Workshop.GoogleMapsLink,
                    OpeningTime = x.Workshop.OpeningTime,
                    ClosingTime = x.Workshop.ClosingTime,
                    BusyUntil = x.Workshop.BusyUntil,
                    StartBusyTime = x.Workshop.BusyFrom,
                    BusyStatus = x.Workshop.IsBusy,
                    DistanceKM = null
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<WorkshopResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
            return Result<PagedResult<WorkshopResponseDto>>.Success(pagedResult, "Saved workshops retrieved successfully");
        }
    }
}