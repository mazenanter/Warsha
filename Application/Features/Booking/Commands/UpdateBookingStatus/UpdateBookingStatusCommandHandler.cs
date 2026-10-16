using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.UpdateBookingStatus
{
    public class UpdateBookingStatusCommandHandler
     : IRequestHandler<UpdateBookingStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UpdateBookingStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(UpdateBookingStatusCommand command, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            var workshop = await _unitOfWork.Workshops.FindAsync(x=>x.UserId == userId);
            if (workshop is null) return Result.Failure("Workshop not found");
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result.Failure("Booking not found");
            if (booking.WorkshopId != workshop.Id) return Result.Failure("Unauthorized");

            booking.UpdateJobStatus(command.NewStatus);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success($"Status updated to {command.NewStatus}");
        }
    }
}
