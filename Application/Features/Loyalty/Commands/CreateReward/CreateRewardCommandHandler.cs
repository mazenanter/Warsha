using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Loyalty.Commands.CreateReward
{
    public class CreateRewardCommandHandler
    : IRequestHandler<CreateRewardCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRewardCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Result<int>> Handle(
            CreateRewardCommand command, CancellationToken ct)
        {
            var reward = LoyaltyReward.Create(
                command.Name, command.Description, command.PointsCost,
                command.DiscountAmount, command.ExpiryDays,
                command.RedemptionLimitPerClient);

            await _unitOfWork.LoyaltyRewards.AddAsync(reward, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<int>.Success(reward.Id, "Reward created successfully");
        }
    }
}
