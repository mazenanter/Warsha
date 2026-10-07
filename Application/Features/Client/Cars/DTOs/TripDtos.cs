using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Client.Cars.DTOs
{
    public record StartTripResult(
     int TripId,
     string Status,
     DateTime StartedAt,
     double StartLatitude,
     double StartLongitude);

    public record CompleteTripResult(
        int TripId,
        string Status,
        DateTime StartedAt,
        DateTime EndedAt,
        decimal DistanceKm,
        decimal NewGpsEstimatedMileageKm);

    public record MileageDto(
     int ActualOdometerKm,
     decimal EstimatedMileageKm,
     decimal GpsAccumulatedKm,
     DateTime? LastUpdatedAt);
}
