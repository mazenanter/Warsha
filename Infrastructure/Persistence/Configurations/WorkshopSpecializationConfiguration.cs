using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopSpecializationConfiguration : IEntityTypeConfiguration<WorkshopSpecialization>
    {
        public void Configure(EntityTypeBuilder<WorkshopSpecialization> builder)
        {
            builder.ToTable("WorkshopSpecializations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkshopId)
                .IsRequired();

            builder.Property(x => x.SpecializationId)
                .IsRequired();

            builder.HasOne(x => x.Workshop)
                .WithMany(x => x.Specializations)
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Specialization)
                .WithMany()
                .HasForeignKey(x => x.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.SpecializationId
            })
            .IsUnique();
        }
    }
}
