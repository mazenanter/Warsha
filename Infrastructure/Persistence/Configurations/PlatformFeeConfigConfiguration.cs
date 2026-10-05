using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PlatformFeeConfigConfiguration
       : IEntityTypeConfiguration<PlatformFeeConfig>
    {
        public void Configure(EntityTypeBuilder<PlatformFeeConfig> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClientBookingFee)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.EffectiveFrom)
                .IsRequired();

            builder.HasIndex(x => x.EffectiveFrom);
        }
    }
}
