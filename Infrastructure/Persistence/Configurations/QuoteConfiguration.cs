using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class QuoteConfiguration
        : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.HasKey(q => q.Id);
            builder.Property(q => q.TotalAmount).HasPrecision(10, 2);
            builder.Property(q => q.Status).HasConversion<string>();
            builder.Property(q => q.WorkshopNote).HasMaxLength(1000);

            builder.HasMany(q => q.Items)
                .WithOne()
                .HasForeignKey(qi => qi.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(q => q.Items).HasField("_items");
        }
    }
}
