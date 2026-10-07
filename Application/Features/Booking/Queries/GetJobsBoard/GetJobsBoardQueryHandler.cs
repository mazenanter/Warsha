using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Queries.GetJobsBoard
{
    public class GetJobsBoardQueryHandler : IRequestHandler<GetJobsBoardQuery, Result<JobsBoardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetJobsBoardQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<JobsBoardDto>> Handle(GetJobsBoardQuery request, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var jobs = await _unitOfWork.Bookings.GetWorkshopActiveJobsAsync(workshopId, ct);

            static JobCardDto ToCard(Domain.Entities.Booking b) => new(
        b.Id, b.BookingNumber, b.Client.Name,
        b.Car.CarModel.CarBrand.Name, b.Car.CarModel.Name,
        string.Join(", ", b.Items.Select(i => i.ServiceName)),
        b.ScheduledAt,
        b.Quotes.Any(q => q.Status == QuoteStatus.Pending));
            var board = new JobsBoardDto(
           jobs.Where(j => j.JobStatus == JobStatus.New)
               .Select(ToCard).ToList(),
           jobs.Where(j => j.JobStatus == JobStatus.Diagnosing)
               .Select(ToCard).ToList(),
           jobs.Where(j => j.JobStatus == JobStatus.InProgress)
               .Select(ToCard).ToList(),
           jobs.Where(j => j.JobStatus == JobStatus.Ready)
               .Select(ToCard).ToList());

            return Result<JobsBoardDto>.Success(board, "Jobs board retrieved successfully");
        }
    }
}
