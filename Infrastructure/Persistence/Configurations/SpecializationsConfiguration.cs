using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SpecializationsConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.ToTable("Specializations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Icon)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.HasOne(x => x.CarBrand)
                .WithMany()
                .HasForeignKey(x => x.CarBrandId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CarModel)
                .WithMany()
                .HasForeignKey(x => x.CarModelId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}