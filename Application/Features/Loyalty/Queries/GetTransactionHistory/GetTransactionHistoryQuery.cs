using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<IEnumerable<LoyaltyTransactionDto>>>;

    public record LoyaltyTransactionDto(
        int Id,
        string Type,
        int Amount,
        int BalanceAfter,
        string Status,
        string Reason,
        DateTime CreatedAt
    );
}
