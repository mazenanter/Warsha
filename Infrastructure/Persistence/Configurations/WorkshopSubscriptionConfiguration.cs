using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopSubscriptionConfiguration
        : IEntityTypeConfiguration<WorkshopSubscription>
    {
        public void Configure(EntityTypeBuilder<WorkshopSubscription> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkshopId)
                .IsRequired();

            builder.Property(x => x.PlanId)
                .IsRequired();

            builder.Property(x => x.RenewsAt)
                .IsRequired();

            builder.HasOne(x => x.Plan)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.WorkshopId);

            builder.HasIndex(x => x.PlanId);

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.PlanId
            });
        }
    }
}
