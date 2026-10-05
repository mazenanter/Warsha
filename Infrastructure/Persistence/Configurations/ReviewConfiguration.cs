

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasOne(r => r.Workshop)
            .WithMany()
            .HasForeignKey(r => r.WorkshopId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.Comment).HasMaxLength(1000);

        }
    }
}