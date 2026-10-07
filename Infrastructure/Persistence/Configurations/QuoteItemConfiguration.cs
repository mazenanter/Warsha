using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class QuoteItemConfiguration
         : IEntityTypeConfiguration<QuoteItem>
    {
        public void Configure(EntityTypeBuilder<QuoteItem> builder)
        {
            builder.HasKey(qi => qi.Id);
            builder.Property(qi => qi.Description).IsRequired().HasMaxLength(200);
            builder.Property(qi => qi.Price).HasPrecision(10, 2);
        }
    }
}
