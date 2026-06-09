using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Infrastructure.Configurations;

public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.ToTable("auth_token");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccessToken)
               .IsRequired();

        builder.Property(x => x.RefreshToken)
               .IsRequired();

        builder.Property(x => x.IsRevoked)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.User)
               .WithMany(x => x.AuthTokens)
               .HasForeignKey(x => x.UserId);
    }
}
