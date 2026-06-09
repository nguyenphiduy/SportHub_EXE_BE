using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Infrastructure.Configurations;

public class AiAnalysisHistoryConfiguration : IEntityTypeConfiguration<AiAnalysisHistory>
{
    public void Configure(EntityTypeBuilder<AiAnalysisHistory> builder)
    {
        builder.ToTable("ai_analysis_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Summary)
               .IsRequired();

        builder.Property(x => x.Trend)
               .IsRequired();

        builder.Property(x => x.Recommendation)
               .IsRequired();

        builder.Property(x => x.EstimatedNextPeriodRevenue)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.AnalyzedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Venue)
               .WithMany()
               .HasForeignKey(x => x.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AnalyzedByUser)
               .WithMany()
               .HasForeignKey(x => x.AnalyzedByUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VenueId);
        builder.HasIndex(x => x.AnalyzedAt);
    }
}
