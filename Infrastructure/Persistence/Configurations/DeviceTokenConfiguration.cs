using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.HasKey(dt => dt.Id);
            builder.Property(dt => dt.Token).IsRequired().HasMaxLength(500);
            builder.Property(dt => dt.Platform).IsRequired().HasMaxLength(10);
            builder.Property(dt => dt.DeviceId)
    .IsRequired()
    .HasMaxLength(200);

            builder.HasIndex(dt => new
            {
                dt.UserId,
                dt.DeviceId
            })
            .IsUnique();

            builder.HasIndex(dt => dt.UserId);
        }
    }
}
