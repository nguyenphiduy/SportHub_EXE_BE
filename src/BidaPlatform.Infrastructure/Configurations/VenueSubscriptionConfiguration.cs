using BidaPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidaPlatform.Infrastructure.Configurations;

public class VenueSubscriptionConfiguration : IEntityTypeConfiguration<VenueSubscription>
{
    public void Configure(EntityTypeBuilder<VenueSubscription> builder)
    {
        builder.ToTable("venue_subscriptions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Plan)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.StartDate)
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(x => x.EndDate)
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(x => x.ApprovedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany(x => x.Subscriptions)
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
