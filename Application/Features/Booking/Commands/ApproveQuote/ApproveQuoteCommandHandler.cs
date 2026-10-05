using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.ApproveQuote
{
    public class ApproveQuoteCommandHandler : IRequestHandler<ApproveQuoteCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ApproveQuoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(ApproveQuoteCommand command, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result.Failure("Booking not found");
            if (booking.ClientId != clientId) return Result.Failure("Unauthorized");

            booking.ApproveQuote(command.QuoteId);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Quote approved. Workshop will proceed.");
        }
    }
}
