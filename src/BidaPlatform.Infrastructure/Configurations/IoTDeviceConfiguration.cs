using BidaPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidaPlatform.Infrastructure.Configurations;

public class IoTDeviceConfiguration : IEntityTypeConfiguration<IoTDevice>
{
    public void Configure(EntityTypeBuilder<IoTDevice> builder)
    {
        builder.ToTable("iot_devices");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TableId).IsUnique();

        builder.Property(x => x.IpAddress)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.DeviceName)
               .HasMaxLength(100);

        builder.Property(x => x.IsOnline)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany(x => x.IoTDevices)
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
