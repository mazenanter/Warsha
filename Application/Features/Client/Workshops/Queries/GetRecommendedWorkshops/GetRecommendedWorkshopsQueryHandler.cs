using Application.Features.Client.Workshops.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Client.Workshops.Queries.GetRecommendedWorkshops
{
    //public class GetRecommendedWorkshopsQueryHandler : IRequestHandler<GetRecommendedWorkshopsQuery, Result<PagedResult<WorkshopResponseDto>>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ICurrentUserService _currentUserService;
    //    public GetRecommendedWorkshopsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _currentUserService = currentUserService;
    //    }

    //    public async Task<Result<PagedResult<WorkshopResponseDto>>> Handle(GetRecommendedWorkshopsQuery request, CancellationToken cancellationToken)
    //    {
    //        var clientId = _currentUserService.ClientId;
    //        if(clientId is null)
    //        {
    //            return Result<PagedResult<WorkshopResponseDto>>.Failure("User Id Doesn't exist");
    //        }
    //        var cars = await _unitOfWork.Cars.GetAll()
    //        .Include(c => c.CarModel)
    //        .Where(c => c.ClientId == clientId.Value)
    //        .ToListAsync(cancellationToken);

    //        if (cars.Count == 0)
    //        {
    //            return Result<PagedResult<WorkshopResponseDto>>.Failure("Add Car First to Get recommendation");
    //        }

    //        var carBrandIds = cars.Select(c => c.CarModel.CarBrandId).Distinct().ToList();
    //        var carModelIds = cars.Select(c => c.CarModelId).Distinct().ToList();

    //        var query = _unitOfWork.Workshops.GetAll()
    //            .Where(w => w.IsVerified && w.Specializations.Any(ws =>
    //                (ws.Specialization.CarBrandId != null && carBrandIds.Contains(ws.Specialization.CarBrandId.Value)) ||
    //                (ws.Specialization.CarModelId != null && carModelIds.Contains(ws.Specialization.CarModelId.Value))))
    //            .OrderByDescending(w => w.RatingAvg);


    //        var totalRecords = await query.CountAsync(cancellationToken);

    //        var data = await query
    //            .Skip((request.PageNumber - 1) * request.PageSize)
    //            .Take(request.PageSize)
    //            .Select(w => new WorkshopResponseDto
    //            {
    //                UserId = w.Id,
    //                Name = w.Name,
    //                AddressEn = w.AddressEn,
    //                AddressAr = w.AddressAr,
    //                ImageUrl = w.ImageUrl,
    //                Phone = w.Phone,
    //                Lat = w.Lat,
    //                Lng = w.Lng,
    //                RatingAvg = w.RatingAvg,
    //                OpeningTime = w.OpeningTime,
    //                ClosingTime = w.ClosingTime,
    //                BusyUntil = w.BusyUntil,
    //                StartBusyTime = w.BusyFrom,
    //                BusyStatus = w.IsBusy,
    //                DistanceKM = null
    //            })
    //            .ToListAsync(cancellationToken);

    //        var pagedResult = PagedResult<WorkshopResponseDto>.Create(data, totalRecords, request.PageNumber, request.PageSize);
    //        return Result<PagedResult<WorkshopResponseDto>>.Success(pagedResult, "Recommended workshops retrieved successfully");

    //    }
    //}

    public class GetRecommendedWorkshopsQueryHandler
        : IRequestHandler<GetRecommendedWorkshopsQuery,
            Result<PagedResult<WorkshopResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        private const double EarthRadiusKm = 6371.0;
        private const double RatingWeight = 0.70;
        private const double DistanceWeight = 0.30;

        public GetRecommendedWorkshopsQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<WorkshopResponseDto>>> Handle(
            GetRecommendedWorkshopsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var clientId= await _unitOfWork.Clients.FindAsync(x=>x.UserId == userId);

            if (clientId is null)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure(
                    "Current user is not associated with a client.");
            }

            if (request.PageNumber < 1 ||
                request.PageSize < 1 ||
                request.PageSize > 100)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure(
                    "Invalid pagination values.");
            }

            if (request.Lat < -90 || request.Lat > 90 ||
                request.Lng < -180 || request.Lng > 180 ||
                request.RadiusKm <= 0 || request.RadiusKm > 100)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure(
                    "Invalid location or search radius.");
            }

            var carBrandIds = await _unitOfWork.Cars.GetAll()
                .Where(c =>
                    c.ClientId == clientId.Id &&
                    !c.IsDeleted)
                .Select(c => c.CarModel.CarBrandId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (carBrandIds.Count == 0)
            {
                return Result<PagedResult<WorkshopResponseDto>>.Failure(
                    "Add a car first to get recommendations.");
            }

            var lat = request.Lat;
            var lng = request.Lng;
            var radiusKm = request.RadiusKm;

            var latDelta = radiusKm / 111.0;
            var cosLat = Math.Cos(ToRadian(lat));

            var lngDelta = radiusKm / (111.0 * Math.Max(
                Math.Abs(cosLat), 0.01));

            var candidates = await _unitOfWork.Workshops.GetAll()
                .Where(w =>
                    w.IsVerified &&
                    w.SupportedCarBrands.Any(b =>
                        carBrandIds.Contains(b.CarBrandId)) &&
                    w.Lat >= lat - latDelta &&
                    w.Lat <= lat + latDelta &&
                    w.Lng >= lng - lngDelta &&
                    w.Lng <= lng + lngDelta)
                .Select(w => new
                {
                    Workshop = w,
                    w.Id,
                    w.Name,
                    w.AddressEn,
                    w.AddressAr,
                    w.ImageUrl,
                    w.Phone,
                    w.Lat,
                    w.Lng,
                    w.RatingAvg,
                    w.OpeningTime,
                    w.ClosingTime,
                    w.BusyUntil,
                    w.BusyFrom,
                    w.IsBusy
                })
                .ToListAsync(cancellationToken);

            var rankedWorkshops = candidates
                .Select(x =>
                {
                    var distance = CalculateDistanceKm(
                        lat, lng, x.Lat, x.Lng);

                    var rating = Math.Clamp(
                        Convert.ToDouble(x.RatingAvg), 0.0, 5.0);

                    var ratingScore = rating / 5.0;

                    var distanceScore = Math.Max(
                        0.0, 1.0 - distance / radiusKm);

                    var recommendationScore =
                        (ratingScore * RatingWeight) +
                        (distanceScore * DistanceWeight);

                    return new
                    {
                        x.Workshop,
                        x.Id,
                        x.Name,
                        x.AddressEn,
                        x.AddressAr,
                        x.ImageUrl,
                        x.Phone,
                        x.Lat,
                        x.Lng,
                        x.RatingAvg,
                        x.OpeningTime,
                        x.ClosingTime,
                        x.BusyUntil,
                        x.BusyFrom,
                        x.IsBusy,
                        Distance = distance,
                        Score = recommendationScore
                    };
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Distance)
                .ThenByDescending(x => x.RatingAvg)
                .ToList();

            var totalRecords = rankedWorkshops.Count;

            var data = rankedWorkshops
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new WorkshopResponseDto
                {
                    UserId = x.Id,
                    Name = x.Name,
                    AddressEn = x.AddressEn,
                    AddressAr = x.AddressAr,
                    ImageUrl = x.ImageUrl,
                    Phone = x.Phone,
                    Lat = x.Lat,
                    Lng = x.Lng,
                    RatingAvg = x.RatingAvg,
                    OpeningTime = x.OpeningTime,
                    ClosingTime = x.ClosingTime,
                    BusyUntil = x.BusyUntil,
                    StartBusyTime = x.BusyFrom,
                    BusyStatus = x.IsBusy,
                    DistanceKM = Math.Round(x.Distance, 2)
                })
                .ToList();

            var pagedResult = PagedResult<WorkshopResponseDto>.Create(
                data,
                totalRecords,
                request.PageNumber,
                request.PageSize);

            return Result<PagedResult<WorkshopResponseDto>>.Success(
                pagedResult,
                "Recommended workshops retrieved successfully.");
        }

        private static double CalculateDistanceKm(
            double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadian(lat2 - lat1);
            var dLon = ToRadian(lon2 - lon1);

            var a =
                Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(ToRadian(lat1)) *
                Math.Cos(ToRadian(lat2)) *
                Math.Pow(Math.Sin(dLon / 2), 2);

            var c = 2 * Math.Atan2(
                Math.Sqrt(a),
                Math.Sqrt(Math.Max(0, 1 - a)));

            return EarthRadiusKm * c;
        }

        private static double ToRadian(double degree)
            => degree * Math.PI / 180.0;
    }
}