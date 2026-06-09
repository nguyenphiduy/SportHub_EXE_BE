using BidaPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidaPlatform.Infrastructure.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Address)
               .HasMaxLength(500);

        builder.Property(x => x.Phone)
               .HasMaxLength(50);

        builder.Property(x => x.OwnerName)
               .HasMaxLength(255);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50)
               .HasDefaultValue(BidaPlatform.Domain.Enums.VenueStatus.Pending);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");
    }
}
