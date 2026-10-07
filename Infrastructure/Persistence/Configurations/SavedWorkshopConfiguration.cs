using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SavedWorkshopConfiguration : IEntityTypeConfiguration<SavedWorkshop>
    {
        public void Configure(EntityTypeBuilder<SavedWorkshop> builder)
        {
            builder.HasIndex(x => new { x.ClientId, x.WorkshopId }).IsUnique();

            builder.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Workshop)
                .WithMany()
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}