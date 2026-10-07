using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PaymentTransactionConfiguration
       : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.PaymentMethod)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.ProviderReference)
                .HasMaxLength(200);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.Property(x => x.ProcessedAt)
                .IsRequired(false);

            builder.HasOne<Booking>()
                .WithMany(x => x.PaymentTransactions)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BookingId);

            builder.HasIndex(x => x.ProviderReference);
        }
    }
}
