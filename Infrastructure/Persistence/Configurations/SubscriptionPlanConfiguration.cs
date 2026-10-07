using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SubscriptionPlanConfiguration
       : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.MonthlyPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.MaxServices)
                .IsRequired();

            builder.Property(x => x.PriorityRanking)
                .IsRequired();

            builder.Property(x => x.AdvancedAnalytics)
                .IsRequired();

            builder.Property(x => x.FeaturedListing)
                .IsRequired();

            builder.Property(x => x.WorkshopCommissionPct)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.WorkshopCancellationFee)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasMany(x => x.Subscriptions)
                .WithOne(x => x.Plan)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
