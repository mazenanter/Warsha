using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Source)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.DistanceKm)
                .HasPrecision(10, 3);

            builder.Property(t => t.StartLatitude)
                .IsRequired();

            builder.Property(t => t.StartLongitude)
                .IsRequired();

            builder.HasOne(t => t.Car)
                .WithMany()
                .HasForeignKey(t => t.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.CarId)
                .IsUnique()
                .HasFilter("[Status] = 'Active'");

            builder.HasIndex(t => new { t.CarId, t.Status });
        }
    }
}
