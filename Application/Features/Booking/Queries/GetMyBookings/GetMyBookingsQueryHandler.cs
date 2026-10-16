using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Queries.GetMyBookings
{
    public class GetMyBookingsQueryHandler
    : IRequestHandler<GetMyBookingsQuery, Result<IEnumerable<BookingListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyBookingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<BookingListDto>>> Handle(
            GetMyBookingsQuery request, CancellationToken ct)
        {
           
            var userId = _currentUser.UserId;
            var client = await _unitOfWork.Clients.FindAsync(x=>x.UserId == userId);
            if(client == null)
            {
                return Result<IEnumerable<BookingListDto>>.Failure("Client not found");
            }
            var bookings = await _unitOfWork.Bookings
                .GetClientBookingsAsync(client.Id, request.Status, request.Page, request.PageSize, ct);

            var result = bookings.Select(b => new BookingListDto(
                b.Id, b.BookingNumber, b.Workshop.Name,
                string.Join(", ", b.Items.Select(i => i.ServiceName)),
                b.ScheduledAt, b.TotalAmount,
                b.BookingStatus.ToString(), b.JobStatus.ToString(), b.PaymentType.ToString()));

            return Result<IEnumerable<BookingListDto>>.Success(result,"Bookings retrieved successfully");
        }
    }
}
