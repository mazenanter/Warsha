using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.CarBrand)
                .WithMany(x => x.CarModels)
                .HasForeignKey(x => x.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.CarBrandId, x.Name })
                .IsUnique();
        }
    }
}
