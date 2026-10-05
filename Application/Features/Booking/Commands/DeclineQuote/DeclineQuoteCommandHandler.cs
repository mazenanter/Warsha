using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.DeclineQuote
{
    public class DeclineQuoteCommandHandler : IRequestHandler<DeclineQuoteCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeclineQuoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(DeclineQuoteCommand command, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result.Failure("Booking not found");
            if (booking.ClientId != clientId) return Result.Failure("Unauthorized");

            booking.DeclineQuote(command.QuoteId);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Quote declined. Workshop will continue with original services only.");
        }
    }
}
