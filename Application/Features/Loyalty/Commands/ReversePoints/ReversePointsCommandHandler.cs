using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Loyalty.Commands.ReversePoints
{
    public class ReversePointsCommandHandler
     : IRequestHandler<ReversePointsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReversePointsCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(
            ReversePointsCommand command, CancellationToken ct)
        {
            var wallet = await _unitOfWork.LoyaltyWallets
                .GetByClientIdAsync(command.ClientId, ct);

            if (wallet is null) return Result.Failure("Wallet not found");

            var transactions = await _unitOfWork.LoyaltyTransactions
                .GetPendingByBookingAsync(command.BookingId, ct);

            var toReverse = transactions
                .Where(t => t.Status is
                    LoyaltyTransactionStatus.Pending or
                    LoyaltyTransactionStatus.Available)
                .ToList();

            foreach (var original in toReverse)
            {
                var points = Math.Abs(original.Amount);

                if (original.Status == LoyaltyTransactionStatus.Pending)
                    wallet.ReversePendingPoints(points);
                else
                    wallet.ReverseAvailablePoints(points);

                var balanceAfter = wallet.TotalPoints;

                var reversal = Domain.Entities.LoyaltyTransaction.CreateReversal(
                    command.ClientId, points, balanceAfter,
                    original.Id, command.Reason, command.BookingId);

                await _unitOfWork.LoyaltyTransactions.AddAsync(reversal, ct);
                original.MarkReversed();
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success("Points reversed successfully");
        }
    }
}
