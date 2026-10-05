using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Queries.GetBookingDetails
{
    public class GetBookingDetailsQueryHandler
    : IRequestHandler<GetBookingDetailsQuery, Result<BookingDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetBookingDetailsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<BookingDetailsDto>> Handle(
            GetBookingDetailsQuery request, CancellationToken ct)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(request.BookingId, ct);
            if (booking is null) return Result<BookingDetailsDto>.Failure("Booking not found");

            var clientId = _currentUser.ClientId;
            var workshopId = _currentUser.WorkshopId;
            if (clientId.HasValue && booking.ClientId != clientId.Value) return Result<BookingDetailsDto>.Failure("Unauthorized");
            if (workshopId.HasValue && booking.WorkshopId != workshopId.Value) return Result<BookingDetailsDto>.Failure("Unauthorized");

            var pendingQuote = booking.Quotes.FirstOrDefault(q => q.Status == QuoteStatus.Pending);

            var dto = new BookingDetailsDto(
                booking.Id, booking.BookingNumber,
                booking.Workshop.Name, booking.Workshop.Address,
                booking.Car.CarModel.CarBrand.Name, booking.Car.CarModel.Name, booking.Car.Year,
                booking.ScheduledAt, booking.TotalAmount, booking.ConfirmationFeeAmount,
                booking.BookingStatus.ToString(), booking.JobStatus.ToString(),
                booking.PaymentType.ToString(), booking.CustomerNotes, booking.CancellationReason,
                booking.ConfirmedAt, booking.CompletedAt,
                booking.Items.Select(i => new BookingServiceDto(i.ServiceName, i.Price)).ToList(),
                pendingQuote == null ? null : new PendingQuoteDto(
                    pendingQuote.Id, pendingQuote.TotalAmount, pendingQuote.WorkshopNote,
                    pendingQuote.Items.Select(i => new QuoteItemDto(i.Description, i.Price)).ToList()));

            return Result<BookingDetailsDto>.Success(dto, "Booking details retrieved successfully");
        }
    }
}
