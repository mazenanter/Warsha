using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopServiceConfiguration : IEntityTypeConfiguration<WorkshopService>
    {
        public void Configure(EntityTypeBuilder<WorkshopService> builder)
        {
            builder.ToTable("WorkshopServices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MinPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.MaxPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.DurationMin)
                .IsRequired();

            builder.Property(x => x.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.DescriptionEn)
                .IsRequired(false)
                .HasMaxLength(500);
            builder.Property(x => x.DescriptionAr)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasOne(x => x.Workshop)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ServiceCategory)
                .WithMany()
                .HasForeignKey(x => x.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.ServiceCategoryId
            });
        }
    }
}
