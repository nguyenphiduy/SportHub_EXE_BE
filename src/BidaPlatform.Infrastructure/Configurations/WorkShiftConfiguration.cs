using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Infrastructure.Configurations;

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.ToTable("work_shifts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.ShiftDate)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.StartTime)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.EndTime)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CheckedInAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany()
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StaffUser)
               .WithMany()
               .HasForeignKey(x => x.StaffUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
