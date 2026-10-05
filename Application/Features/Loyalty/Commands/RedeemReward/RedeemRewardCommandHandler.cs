using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Loyalty.Commands.RedeemReward
{
    public class RedeemRewardCommandHandler
    : IRequestHandler<RedeemRewardCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public RedeemRewardCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<string>> Handle(
            RedeemRewardCommand command, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;

            var reward = await _unitOfWork.LoyaltyRewards
                .GetByIdAsync(command.RewardId, ct);

            if (reward is null || !reward.IsAvailable())
                return Result<string>.Failure("Reward not available");

            var wallet = await _unitOfWork.LoyaltyWallets
                .GetByClientIdAsync(clientId, ct);

            if (wallet is null)
                return Result<string>.Failure("Loyalty wallet not found");

            if (wallet.AvailablePoints < reward.PointsCost)
                return Result<string>.Failure(
                    $"Insufficient points. You have {wallet.AvailablePoints} WP, need {reward.PointsCost} WP");

            if (reward.RedemptionLimitPerClient.HasValue)
            {
                var count = await _unitOfWork.LoyaltyRewards
                    .GetClientRedemptionCountAsync(clientId, command.RewardId, ct);

                if (count >= reward.RedemptionLimitPerClient.Value)
                    return Result<string>.Failure("Redemption limit reached for this reward");
            }

            wallet.DeductPoints(reward.PointsCost);

            var balanceAfter = wallet.AvailablePoints;

            var transaction = LoyaltyTransaction.CreateRedeem(
                clientId, reward.PointsCost, balanceAfter,
                reward.Id, $"Redeemed: {reward.Name}");

            await _unitOfWork.LoyaltyTransactions.AddAsync(transaction, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var voucher = RewardVoucher.Create(
                clientId, reward.Id, transaction.Id, reward.ExpiryDays);

            await _unitOfWork.RewardVouchers.AddAsync(voucher, ct);

            transaction.SetVoucherId(voucher.Id);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Success(voucher.VoucherCode,
                $"Reward redeemed! Your voucher: {voucher.VoucherCode}");
        }
    }
}
