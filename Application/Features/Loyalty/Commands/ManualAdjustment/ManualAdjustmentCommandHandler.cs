using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.ManualAdjustment
{
    public class ManualAdjustmentCommandHandler
    : IRequestHandler<ManualAdjustmentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ManualAdjustmentCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            ManualAdjustmentCommand command, CancellationToken ct)
        {
            var wallet = await _unitOfWork.LoyaltyWallets
                .GetByClientIdAsync(command.ClientId, ct);

            if (wallet is null)
                return Result.Failure("Loyalty wallet not found");

            if (command.Amount > 0)
            {
                wallet.AddPendingPoints(command.Amount);
                wallet.MakeAvailable(command.Amount);
            }
            else
            {
                var points = Math.Abs(command.Amount);
                if (wallet.AvailablePoints < points)
                    return Result.Failure(
                        $"Cannot deduct {points} WP, client has {wallet.AvailablePoints} WP");

                wallet.DeductPoints(points);
            }

            var transaction = Domain.Entities.LoyaltyTransaction
                .CreateManualAdjustment(
                    command.ClientId, command.Amount,
                    wallet.TotalPoints, command.Reason,
                    _currentUser.UserId);

            await _unitOfWork.LoyaltyTransactions.AddAsync(transaction, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Points adjusted successfully");
        }
    }
}
