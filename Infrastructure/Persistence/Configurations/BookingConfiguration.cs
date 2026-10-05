using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>

    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.BookingNumber).IsRequired().HasMaxLength(30);
            builder.HasIndex(b => b.BookingNumber).IsUnique();
            builder.HasIndex(b => b.PaymentOrderId)
                .IsUnique()
                .HasFilter("[PaymentOrderId] IS NOT NULL");

            builder.Property(b => b.TotalAmount).HasPrecision(10, 2);
            builder.Property(b => b.ConfirmationFeeAmount).HasPrecision(10, 2);
            builder.Property(b => b.CommissionAmount).HasPrecision(10, 2);
            builder.Property(b => b.WorkshopCancellationFee).HasPrecision(10, 2);
            builder.Property(b => b.BookingStatus).HasConversion<string>();
            builder.Property(b => b.JobStatus).HasConversion<string>();
            builder.Property(b => b.PaymentType).HasConversion<string>();
            builder.Property(b => b.CustomerNotes).HasMaxLength(500);
            builder.Property(b => b.CancellationReason).HasMaxLength(500);

            builder.HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Quotes)
                .WithOne()
                .HasForeignKey(q => q.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.PaymentTransactions)
                .WithOne()
                .HasForeignKey(pt => pt.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Fees)
                .WithOne()
                .HasForeignKey(bf => bf.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Client)
                .WithMany()
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Workshop)
                .WithMany()
                .HasForeignKey(b => b.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Car)
                .WithMany()
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(b => b.Items).HasField("_items");
            builder.Navigation(b => b.Quotes).HasField("_quotes");
            builder.Navigation(b => b.PaymentTransactions).HasField("_paymentTransactions");
            builder.Navigation(b => b.Fees).HasField("_fees");
        }
    }
}
