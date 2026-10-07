using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Loyalty.Queries.GetMyVouchers
{
    public record GetMyVouchersQuery(string? Status = null)
    : IRequest<Result<IEnumerable<VoucherDto>>>;

    public record VoucherDto(
        int Id,
        string VoucherCode,
        string RewardName,
        string RewardDescription,
        int PointsCost,
        decimal? DiscountAmount,
        string Status,
        DateTime CreatedAt,
        DateTime? ExpiresAt,
        DateTime? UsedAt,
        bool IsExpired,
        int? DaysUntilExpiry 
    );
}
