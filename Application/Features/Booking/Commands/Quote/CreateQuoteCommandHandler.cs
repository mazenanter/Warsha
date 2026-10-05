using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Commands.Quote
{
    public class CreateQuoteCommandHandler : IRequestHandler<CreateQuoteCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateQuoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(CreateQuoteCommand command, CancellationToken ct)
        {
            var workshopId = _currentUser.WorkshopId!.Value;
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(command.BookingId, ct);

            if (booking is null) return Result<int>.Failure("Booking not found");
            if (booking.WorkshopId != workshopId) return Result<int>.Failure("Unauthorized");

            var items = command.Items.Select(i => (i.Description, i.Price)).ToList();
            var quote = booking.CreateQuote(items, command.Note);

            await _unitOfWork.SaveChangesAsync(ct);
            return Result<int>.Success(quote.Id, "Quote sent to client");
        }
    }
}
