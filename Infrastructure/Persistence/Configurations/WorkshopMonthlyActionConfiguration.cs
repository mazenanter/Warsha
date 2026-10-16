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
    public class WorkshopMonthlyActionConfiguration
     : IEntityTypeConfiguration<WorkshopMonthlyAction>
    {
        public void Configure(
            EntityTypeBuilder<WorkshopMonthlyAction> builder)
        {
            builder.ToTable("WorkshopMonthlyActions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkshopId)
                .IsRequired();

            builder.Property(x => x.BookingId)
                .IsRequired();

            builder.Property(x => x.Year)
                .IsRequired();

            builder.Property(x => x.Month)
                .IsRequired();

            builder.Property(x => x.ActionType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.PenaltyAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.BookingId,
                x.ActionType
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.WorkshopId,
                x.Year,
                x.Month
            });

            builder.HasOne<Workshop>()
                .WithMany()
                .HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Booking>()
                .WithMany()
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
    }
