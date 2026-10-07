using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingServiceConfiguration
        : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.ServiceName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(bs => bs.Price)
                .HasPrecision(10, 2);

            builder.HasOne(bs => bs.Booking)
                .WithMany(b => b.Items)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bs => bs.WorkshopService)
                .WithMany()
                .HasForeignKey(bs => bs.WorkshopServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
