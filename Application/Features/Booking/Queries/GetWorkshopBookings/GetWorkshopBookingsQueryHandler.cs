using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetWorkshopBookings
{
    public class GetWorkshopBookingsQueryHandler
     : IRequestHandler<GetWorkshopBookingsQuery, Result<IEnumerable<BookingListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetWorkshopBookingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<BookingListDto>>> Handle(
            GetWorkshopBookingsQuery request, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var bookings = await _unitOfWork.Bookings
                .GetWorkshopBookingsAsync(workshopId, request.Page, request.PageSize, ct);

            var result = bookings.Select(b => new BookingListDto(
                b.Id, b.BookingNumber, b.Client.Name,
                string.Join(", ", b.Items.Select(i => i.ServiceName)),
                b.ScheduledAt, b.TotalAmount,
                b.BookingStatus.ToString(), b.JobStatus.ToString(), b.PaymentType.ToString()));

            return Result<IEnumerable<BookingListDto>>.Success(result, "Workshop bookings retrieved successfully");
        }
    }
}
