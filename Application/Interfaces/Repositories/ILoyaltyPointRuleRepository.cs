using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
    public interface ILoyaltyPointRuleRepository : IRepository<LoyaltyPointRule>
    {
        Task<LoyaltyPointRule?> GetByActionAsync(
            LoyaltyAction action, CancellationToken ct = default);

        Task<IEnumerable<LoyaltyPointRule>> GetAllActiveAsync(
            CancellationToken ct = default);
    }
}
