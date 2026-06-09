using BidaPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidaPlatform.Infrastructure.Configurations;

public class BilliardTableConfiguration : IEntityTypeConfiguration<BilliardTable>
{
    public void Configure(EntityTypeBuilder<BilliardTable> builder)
    {
        builder.ToTable("billiard_tables");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Type)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.PricePerHour)
               .HasColumnType("numeric(15,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany(x => x.Tables)
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.VenueId, x.Name }).IsUnique();

        builder.HasOne(x => x.IoTDevice)
               .WithOne(x => x.Table)
               .HasForeignKey<IoTDevice>(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Sessions)
               .WithOne(x => x.Table)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
