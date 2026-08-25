using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
    {
        public void Configure(EntityTypeBuilder<Workshop> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.UserId).IsRequired();
            builder.Property(w => w.Name).IsRequired().HasMaxLength(150);
            builder.Property(w => w.Phone).IsRequired().HasMaxLength(20);
            builder.Property(w => w.Address).IsRequired().HasMaxLength(300);
            builder.Property(w => w.RatingAvg).HasPrecision(3, 2);
            builder.HasIndex(w => w.UserId).IsUnique();
            builder.HasMany(x => x.Services)
    .WithOne(x => x.Workshop)
    .HasForeignKey(x => x.WorkshopId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Services)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
