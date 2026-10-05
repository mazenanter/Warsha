using Domain.Common;

namespace Domain.Entities
{
    public class WorkshopSubscription : BaseEntity
    {
        public int WorkshopId { get; private set; }
        public int PlanId { get; private set; }
        public SubscriptionPlan Plan { get; private set; } = default!;
        public DateTime RenewsAt { get; private set; }

        protected WorkshopSubscription() { }

        public static WorkshopSubscription Create(int workshopId, int planId)
        {
            if (workshopId <= 0)
                throw new DomainException("Invalid workshop ID");

            if (planId <= 0)
                throw new DomainException("Invalid plan ID");

            return new WorkshopSubscription
            {
                WorkshopId = workshopId,
                PlanId = planId,
                RenewsAt = DateTime.UtcNow.AddMonths(1),
               
            };
        }

       

        public void ChangePlan(int newPlanId)
        {
            if (newPlanId <= 0)
                throw new DomainException("Invalid plan ID");

            PlanId = newPlanId;
            RenewsAt = DateTime.UtcNow.AddMonths(1);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Renew()
        {
            RenewsAt = DateTime.UtcNow.AddMonths(1);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
