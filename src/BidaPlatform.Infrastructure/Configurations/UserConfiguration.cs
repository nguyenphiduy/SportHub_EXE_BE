using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", t =>
        {
            t.HasCheckConstraint(
                "CK_users_role_venue_scope",
                "(\"Role\" = 'SuperAdmin' AND \"VenueId\" IS NULL) OR (\"Role\" IN ('Manager','Staff') AND \"VenueId\" IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Password)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Role)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany(x => x.Users)
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.ManagedVenues)
               .WithOne(x => x.PrimaryManager)
               .HasForeignKey(x => x.PrimaryManagerId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.ApprovedSubscriptions)
               .WithOne(x => x.ApprovedBySuperAdmin)
               .HasForeignKey(x => x.ApprovedBySuperAdminId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.AuthTokens)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId);

    }
}
