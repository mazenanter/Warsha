using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetMyWallet
{
    public class GetMyWalletQueryHandler
     : IRequestHandler<GetMyWalletQuery, Result<WalletDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyWalletQueryHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<WalletDto>> Handle(
            GetMyWalletQuery request, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;

            var wallet = await _unitOfWork.LoyaltyWallets
                .GetByClientIdAsync(clientId, ct);

            if (wallet is null)
                return Result<WalletDto>.Failure("Wallet not found");

            var rewards = await _unitOfWork.LoyaltyRewards.GetActiveAsync(ct);

            var nextReward = rewards
                .Where(r => r.PointsCost > wallet.AvailablePoints)
                .OrderBy(r => r.PointsCost)
                .FirstOrDefault();

            LoyaltyRewardProgressDto? progress = null;
            if (nextReward != null)
            {
                var remaining = nextReward.PointsCost - wallet.AvailablePoints;
                var percent = (double)wallet.AvailablePoints / nextReward.PointsCost * 100;
                progress = new LoyaltyRewardProgressDto(
                    nextReward.Name, nextReward.PointsCost, remaining,
                    Math.Round(percent, 1));
            }

            return Result<WalletDto>.Success(new WalletDto(
                wallet.AvailablePoints,
                wallet.PendingPoints,
                wallet.LifetimeEarned,
                wallet.LifetimeRedeemed,
                progress), "Wallet retrieved successfully");
        }
    }
}
