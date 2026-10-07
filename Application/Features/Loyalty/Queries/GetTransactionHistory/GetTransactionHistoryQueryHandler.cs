using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery, Result<IEnumerable<LoyaltyTransactionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetTransactionHistoryQueryHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<LoyaltyTransactionDto>>> Handle(
            GetTransactionHistoryQuery request, CancellationToken ct)
        {
            var clientId = _currentUser.ClientId!.Value;

            var transactions = await _unitOfWork.LoyaltyTransactions
                .GetByClientIdAsync(clientId, request.Page, request.PageSize, ct);

            var result = transactions.Select(t => new LoyaltyTransactionDto(
                t.Id,
                t.Type.ToString(),
                t.Amount,
                t.BalanceAfter,
                t.Status.ToString(),
                t.Reason,
                t.CreatedAt));

            return Result<IEnumerable<LoyaltyTransactionDto>>.Success(result,"Transaction history retrieved successfully.");
        }
    }
}
