using BidaPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidaPlatform.Infrastructure.Configurations;

public class BilliardSessionConfiguration : IEntityTypeConfiguration<BilliardSession>
{
    public void Configure(EntityTypeBuilder<BilliardSession> builder)
    {
        builder.ToTable("billiard_sessions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartTime)
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(x => x.EndTime)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.TotalPrice)
               .HasColumnType("numeric(15,2)");

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Note)
               .HasMaxLength(500);

        builder.Property(x => x.PaymentMethod)
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.PaymentStatus)
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany(x => x.Sessions)
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Table)
               .WithMany(x => x.Sessions)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StartedByUser)
               .WithMany()
               .HasForeignKey(x => x.StartedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
