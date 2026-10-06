using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetMyVouchers
{
    public class GetMyVouchersQueryHandler
      : IRequestHandler<GetMyVouchersQuery, Result<IEnumerable<VoucherDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyVouchersQueryHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<VoucherDto>>> Handle(
            GetMyVouchersQuery request, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;

            var vouchers = await _unitOfWork.RewardVouchers
                .GetByClientIdAsync(clientId, ct);

            if (!string.IsNullOrEmpty(request.Status) &&
                Enum.TryParse<RewardVoucherStatus>(request.Status, true, out var statusFilter))
            {
                vouchers = vouchers.Where(v => v.Status == statusFilter);
            }

            var now = DateTime.UtcNow;
            var result = vouchers
                .OrderByDescending(v => v.CreatedAt)
                .Select(v =>
                {
                    var isExpired = v.ExpiresAt.HasValue && now > v.ExpiresAt.Value;
                    var daysUntilExp = v.ExpiresAt.HasValue && v.Status == RewardVoucherStatus.Available && !isExpired
                        ? (int?)Math.Ceiling((v.ExpiresAt.Value - now).TotalDays)
                        : null;

                    var displayStatus = isExpired && v.Status == RewardVoucherStatus.Available
                        ? RewardVoucherStatus.Expired.ToString()
                        : v.Status.ToString();

                    return new VoucherDto(
                        v.Id,
                        v.VoucherCode,
                        v.Reward.Name,
                        v.Reward.Description,
                        v.Reward.PointsCost,
                        v.Reward.DiscountAmount,
                        displayStatus,
                        v.CreatedAt,
                        v.ExpiresAt,
                        v.UsedAt,
                        isExpired,
                        daysUntilExp);
                });

            return Result<IEnumerable<VoucherDto>>.Success(result, "Vouchers retrieved successfully");
        }
    }
}
