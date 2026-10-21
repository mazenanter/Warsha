using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Workshop.Commands.Quotes.WithdrawQuote
{
    public class WithdrawQuoteCommandHandler : IRequestHandler<WithdrawQuoteCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public WithdrawQuoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(WithdrawQuoteCommand request, CancellationToken cancellationToken)
        {
            // 1. Resolve the workshop that owns the quote (via the booking).
            var userId = _currentUserService.UserId;

            var workshop = await _unitOfWork.Workshops
                .GetByUserIdAsync(userId, cancellationToken);

            if (workshop is null)
                return Result.Failure("Workshop not found.");

            // 2. Load the quote.
            var quote = await _unitOfWork.Quotes
                .GetByIdAsync(request.QuoteId, cancellationToken);

            if (quote is null)
                return Result.Failure("Quote not found.");

            // 3. Make sure the quote belongs to a booking owned by this workshop
            //    so a workshop cannot withdraw another workshop's quote.
            var booking = await _unitOfWork.Bookings
                .GetByIdAsync(quote.BookingId, cancellationToken);

            if (booking is null || booking.WorkshopId != workshop.Id)
                return Result.Failure("Quote not found.");

            // 4. Apply the domain rule: only pending quotes can be withdrawn.
            if (quote.Status != QuoteStatus.Pending)
                return Result.Failure("Only pending quotes can be withdrawn.");

            try
            {
                quote.Withdraw();
            }
            catch (DomainException ex)
            {
                // Defensive — the guard above already covers this,
                // but the entity is the source of truth for the rule.
                return Result.Failure(ex.Message);
            }

            // 5. Persist. The Quote is tracked by the DbContext so
            //    SaveChangesAsync is enough; no explicit Update call needed.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Quote withdrawn successfully.");
        }
    }
}