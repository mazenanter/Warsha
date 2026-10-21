using Application.Features.Booking.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BookingEntity = Domain.Entities.Booking;

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

            var clientId = _currentUser.ClientId;
            var workshopId = _currentUser.WorkshopId;

            // Missing booking and someone else's booking get the same answer, so ids can't be probed
            if (booking is null || !IsOwner(booking, clientId, workshopId))
                return Result<BookingDetailsDto>.Failure("Booking not found");

            var isWorkshopOwner = workshopId.HasValue && booking.WorkshopId == workshopId.Value;

            // Separate small query: I haven't seen what GetByIdWithDetailsAsync includes
            BookingCustomerDto? customer = null;
            if (isWorkshopOwner)
            {
                customer = await _unitOfWork.Clients.GetAll()
                    .AsNoTracking()
                    .Where(c => c.Id == booking.ClientId)
                    .Select(c => new BookingCustomerDto(c.Name, c.PhoneNumber))
                    .FirstOrDefaultAsync(ct);
            }

            // Jobs move strictly one step at a time, so the next valid step is the next number
            int? nextJobStatus = isWorkshopOwner
                && booking.BookingStatus == BookingStatus.Confirmed
                && booking.JobStatus != JobStatus.Completed
                    ? (int)booking.JobStatus + 1
                    : null;

            var pendingQuote = booking.Quotes.FirstOrDefault(q => q.Status == QuoteStatus.Pending);

            var dto = new BookingDetailsDto(
                booking.Id, booking.BookingNumber,
                booking.Workshop.Name, booking.Workshop.AddressEn,
                booking.Workshop.AddressAr,
                booking.Car.CarModel.CarBrand.Name, booking.Car.CarModel.Name, booking.Car.Year,
                booking.ScheduledAt, booking.TotalAmount, booking.ConfirmationFeeAmount,
                booking.BookingStatus.ToString(), booking.JobStatus.ToString(),
                booking.PaymentType.ToString(), booking.CustomerNotes, booking.CancellationReason,
                booking.ConfirmedAt, booking.CompletedAt,
                booking.Items.Select(i => new BookingServiceDto(i.ServiceName, i.Price)).ToList(),
                pendingQuote == null ? null : new PendingQuoteDto(
                    pendingQuote.Id, pendingQuote.TotalAmount, pendingQuote.WorkshopNote,
                    pendingQuote.Items.Select(i => new QuoteItemDto(i.Description, i.Price)).ToList()),
                customer,
                nextJobStatus);

            return Result<BookingDetailsDto>.Success(dto, "Booking details retrieved successfully");
        }

        private static bool IsOwner(BookingEntity booking, int? clientId, int? workshopId)
            => (workshopId.HasValue && booking.WorkshopId == workshopId.Value)
            || (clientId.HasValue && booking.ClientId == clientId.Value);
    }
}