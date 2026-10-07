using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OdometerCorrectionConfiguration
        : IEntityTypeConfiguration<OdometerCorrection>
    {
        public void Configure(EntityTypeBuilder<OdometerCorrection> builder)
        {
            builder.HasKey(oc => oc.Id);

            builder.Property(oc => oc.Note)
                .HasMaxLength(300);

            builder.HasOne<Car>()
                .WithMany(c => c.Corrections)
                .HasForeignKey(oc => oc.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(oc => oc.CarId);
        }
    }
}
