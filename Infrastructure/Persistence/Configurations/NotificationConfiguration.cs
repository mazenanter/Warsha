using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).IsRequired().HasMaxLength(100);
            builder.Property(n => n.Body).IsRequired().HasMaxLength(500);
            builder.Property(n => n.RecipientType).IsRequired().HasMaxLength(20);
            builder.Property(n => n.Type).HasConversion<string>();
            builder.HasIndex(n => n.RecipientUserId);
        }
    }
}
