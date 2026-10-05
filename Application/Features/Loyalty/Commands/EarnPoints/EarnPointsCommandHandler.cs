using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Loyalty.Commands.EarnPoints
{
    public class EarnPointsCommandHandler : IRequestHandler<EarnPointsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EarnPointsCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(EarnPointsCommand command, CancellationToken ct)
        {
            var rule = await _unitOfWork.LoyaltyPointRules
                .GetByActionAsync(command.Action, ct);

            if (rule is null || !rule.IsActive)
                return Result.Success("Points earned successfully");

            var wallet = await _unitOfWork.LoyaltyWallets
                .GetByClientIdAsync(command.ClientId, ct);

            if (wallet is null)
                return Result.Failure("Loyalty wallet not found");

            if (rule.IsOneTime)
            {
                var alreadyEarned = await _unitOfWork.LoyaltyTransactions
                    .HasEarnedForActionAsync(command.ClientId, command.Action, ct);

                if (alreadyEarned) return Result.Success("Points earned successfully");
            }

            if (rule.FrequencyDays.HasValue)
            {
                var lastEarn = await _unitOfWork.LoyaltyTransactions
                    .GetLastByActionAsync(command.ClientId, command.Action, ct);

                if (lastEarn != null &&
                    (DateTime.UtcNow - lastEarn.CreatedAt).TotalDays < rule.FrequencyDays.Value)
                    return Result.Success("Points earned successfully");
            }

            wallet.AddPendingPoints(rule.Points);

            var balanceAfter = wallet.PendingPoints + wallet.AvailablePoints;

            var transaction = command.Action is
                LoyaltyAction.FirstBooking or
                LoyaltyAction.FriendFirstBooking or
                LoyaltyAction.Campaign or
                LoyaltyAction.PartnerPromotion
                ? LoyaltyTransaction.CreateBonus(
                    command.ClientId, rule.Points, balanceAfter,
                    command.Action, command.Reason, command.BookingId)
                : LoyaltyTransaction.CreateEarn(
                    command.ClientId, rule.Points, balanceAfter,
                    command.Action, command.Reason, command.BookingId);

            await _unitOfWork.LoyaltyTransactions.AddAsync(transaction, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Points earned successfully");
        }
    }
}
