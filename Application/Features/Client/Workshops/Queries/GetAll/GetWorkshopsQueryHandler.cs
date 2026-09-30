using Application.Features.Client.Workshops.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Workshops.Queries.GetAll
{
    public class GetWorkshopsQueryHandelr : IRequestHandler<GetWorkshopsQuery, Result<PagedResult<WorkshopResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private const double EarthRadiusKm = 6371.0;
        public GetWorkshopsQueryHandelr(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<PagedResult<WorkshopResponseDto>>> Handle(GetWorkshopsQuery request, CancellationToken cancellationToken)
        {
            var Query = _unitOfWork.Workshops.GetAll()
            .Where(w => w.IsVerified);

            if(!string.IsNullOrEmpty(request.SearchTerm))
            {
                Query = Query.Where(w => w.Name.Contains(request.SearchTerm));
            }

            if (request.Offers)
            {
                var today = DateTime.UtcNow;

                Query = Query.Where(
                    w => w.Offers.Any(
                        o => o.IsActive &&
                        o.StartAt <= today && 
                        o.EndAt >= today
                    )
                );
            }

            if (request.NearMe)
            {
                return await HandleNearme(Query, request, cancellationToken);
            }

            Query = Query.OrderByDescending(w => w.RatingAvg).ThenBy(w => w.Name);

            var TotalCount = await Query.CountAsync(cancellationToken);

            var page = await Query.Skip((request.PageNumber - 1) * request.PageSize)
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
                DistanceKM = null
            }).ToListAsync(cancellationToken);


            var pagedResult = PagedResult<WorkshopResponseDto>.Create(page, TotalCount, request.PageNumber, request.PageSize);
            return Result<PagedResult<WorkshopResponseDto>>.Success(pagedResult, "pages retrieved successfully");

        }


    public async Task<Result<PagedResult<WorkshopResponseDto>>> HandleNearme(
        IQueryable<Domain.Entities.Workshop> Query,
        GetWorkshopsQuery response,
        CancellationToken cancellationToken
    )
    {
        var lat = response.Lat;
        var lng = response.Lng;
        var RadiusKm = response.RadiusKm ?? 10;

        var latDelta = RadiusKm / 111.0;
        var lngDelta = RadiusKm / (111.0 * Math.Cos(ToRadian(lat)));

        Query = Query.Where(w => 
        w.Lat >= lat - latDelta && w.Lat <= lat + latDelta &&
        w.Lng >= lng - lngDelta && w.Lng <= lng + lngDelta);

        var candidates = await Query.ToListAsync(cancellationToken);

        var WithDistance = candidates
        .Select(w => new{ workshop = w, Distance = CalculateDistanceKm(lat, lng, w.Lat, w.Lng)})
        .Where(x => x.Distance <= RadiusKm)
        .OrderBy(x => x.Distance)
        .ToList();


        var page = WithDistance.Skip((response.PageNumber - 1) * response.PageSize)
        .Select(x => new WorkshopResponseDto
        {
            UserId = x.workshop.Id,
            Name = x.workshop.Name,
            Address = x.workshop.Address,
            Phone = x.workshop.Phone,
            Lat = x.workshop.Lat,
            Lng = x.workshop.Lng,
            RatingAvg = x.workshop.RatingAvg,
            GoogleMapsLink = x.workshop.GoogleMapsLink,
            OpeningTime = x.workshop.OpeningTime,
            ClosingTime = x.workshop.ClosingTime,
            BusyDuration = x.workshop.BusyDuration,
            StartBusyTime = x.workshop.StartBusyTime,
            BusyStatus = x.workshop.BusyStatus,
            DistanceKM = Math.Round(x.Distance, 2)
        }).ToList();

        var TotalCount = WithDistance.Count();

        var pagedResult1 = PagedResult<WorkshopResponseDto>.Create(page, TotalCount, response.PageNumber, response.PageSize);
        return Result<PagedResult<WorkshopResponseDto>>.Success(pagedResult1, "pages retrieved successfully");

    }


    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRadian(lat2 - lat1);
        var dLon = ToRadian(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }
    private static double ToRadian(double degree)
    {
        return degree * Math.PI / 180;
    }

    }
}