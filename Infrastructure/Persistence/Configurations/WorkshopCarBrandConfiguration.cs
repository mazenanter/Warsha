using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopCarBrandConfiguration : IEntityTypeConfiguration<WorkshopCarBrand>
    {
        public void Configure(EntityTypeBuilder<WorkshopCarBrand> builder)
        {
            builder.ToTable("WorkshopCarBrands");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Workshop)
                .WithMany(x => x.SupportedCarBrands)
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CarBrand)
                .WithMany(x => x.SupportedByWorkshops)
                .HasForeignKey(x => x.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.CarBrandId
            }).IsUnique();
        }
    }
}
