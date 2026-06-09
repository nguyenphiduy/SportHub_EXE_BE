using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Infrastructure.Configurations;

public class AiProviderSettingsConfiguration : IEntityTypeConfiguration<AiProviderSettings>
{
    public void Configure(EntityTypeBuilder<AiProviderSettings> builder)
    {
        builder.ToTable("ai_provider_settings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.EncryptedApiKey)
               .IsRequired();

        builder.Property(x => x.Model)
               .IsRequired()
               .HasMaxLength(200)
               .HasDefaultValue("openrouter/free");

        builder.Property(x => x.BaseUrl)
               .IsRequired()
               .HasMaxLength(500)
               .HasDefaultValue("https://openrouter.ai/api/v1");

        builder.Property(x => x.IsEnabled)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.UpdatedByUser)
               .WithMany()
               .HasForeignKey(x => x.UpdatedByUserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ProviderName).IsUnique();
    }
}
