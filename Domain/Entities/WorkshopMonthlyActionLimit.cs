using Domain.Common;

namespace Domain.Entities
{
    public class WorkshopMonthlyActionLimit : BaseEntity
    {
        private const int FreeActionsLimit = 5;

        public int WorkshopId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }

        public int ActionsCount { get; private set; }

        protected WorkshopMonthlyActionLimit() { }

        public static WorkshopMonthlyActionLimit Create(
            int workshopId,
            int year,
            int month)
        {
            if (workshopId <= 0)
                throw new DomainException(
                    "Workshop ID must be positive.");

            if (year < 2000)
                throw new DomainException(
                    "Invalid year.");

            if (month is < 1 or > 12)
                throw new DomainException(
                    "Month must be between 1 and 12.");

            return new WorkshopMonthlyActionLimit
            {
                WorkshopId = workshopId,
                Year = year,
                Month = month,
                ActionsCount = 0
            };
        }

        public WorkshopActionDecision RegisterAction(
            decimal confirmationFeeAmount)
        {
            if (confirmationFeeAmount < 0)
                throw new DomainException(
                    "Confirmation fee cannot be negative.");

            ActionsCount++;

            var penaltyRequired = ActionsCount > FreeActionsLimit;

            var penaltyAmount = penaltyRequired
                ? confirmationFeeAmount
                : 0m;

            return new WorkshopActionDecision(
                ActionsCount,
                penaltyRequired,
                penaltyAmount);
        }
    }

    public sealed record WorkshopActionDecision(
        int ActionNumber,
        bool PenaltyApplied,
        decimal PenaltyAmount);
}
