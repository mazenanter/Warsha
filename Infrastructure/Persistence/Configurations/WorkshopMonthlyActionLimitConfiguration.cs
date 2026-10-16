using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkshopMonthlyActionLimitConfiguration
     : IEntityTypeConfiguration<WorkshopMonthlyActionLimit>
    {
        public void Configure(
            EntityTypeBuilder<WorkshopMonthlyActionLimit> builder)
        {
            builder.ToTable("WorkshopMonthlyActionLimits");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkshopId)
                .IsRequired();

            builder.Property(x => x.Year)
                .IsRequired();

            builder.Property(x => x.Month)
                .IsRequired();

            builder.Property(x => x.ActionsCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.Year,
                x.Month
            })
            .IsUnique();

            builder.HasOne<Workshop>()
                .WithMany()
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
