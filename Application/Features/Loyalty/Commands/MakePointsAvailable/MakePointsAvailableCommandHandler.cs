using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.MakePointsAvailable
{
    public class MakePointsAvailableCommandHandler
      : IRequestHandler<MakePointsAvailableCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MakePointsAvailableCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(
            MakePointsAvailableCommand command, CancellationToken ct)
        {
            var pendingTransactions = await _unitOfWork.LoyaltyTransactions
                .GetPendingByBookingAsync(command.BookingId, ct);

            foreach (var transaction in pendingTransactions)
            {
                var wallet = await _unitOfWork.LoyaltyWallets
                    .GetByClientIdAsync(transaction.ClientId, ct);

                if (wallet is null) continue;

                wallet.MakeAvailable(Math.Abs(transaction.Amount));
                transaction.MakeAvailable();
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success("Points made available successfully");
        }
    }
}
