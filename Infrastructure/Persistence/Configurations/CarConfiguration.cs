using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Year)
                .IsRequired();

            builder.Property(x => x.CurrentKm)
                .IsRequired(false);

            builder.Property(x => x.LicensePlate)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.HasOne(x => x.Client)
                .WithMany(x => x.Cars)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CarModel)
                .WithMany(x => x.Cars)
                .HasForeignKey(x => x.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ClientId);
            builder.HasIndex(x => x.CarModelId);
        }
    }
}
