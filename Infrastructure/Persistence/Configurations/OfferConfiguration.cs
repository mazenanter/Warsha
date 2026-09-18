using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DiscountPercentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.StartAt)
                .IsRequired();

            builder.Property(x => x.EndAt)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Workshop)
                .WithMany(w => w.Offers)
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.WorkshopService)
                .WithMany(s => s.Offers)
                .HasForeignKey(x => x.WorkshopServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.WorkshopId);
            builder.HasIndex(x => x.WorkshopServiceId);
        }
    }
}
