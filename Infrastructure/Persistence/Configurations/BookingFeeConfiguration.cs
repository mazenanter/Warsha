using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingFeeConfiguration
       : IEntityTypeConfiguration<BookingFee>
    {
        public void Configure(EntityTypeBuilder<BookingFee> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FeeType)
                .IsRequired();

            builder.Property(bf => bf.Amount).HasPrecision(10, 2);


            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.PaidAt)
                .IsRequired(false);

            builder.HasOne<Booking>()
                .WithMany(x => x.Fees)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BookingId);
        }
    }
}
