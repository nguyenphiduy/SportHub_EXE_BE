using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Infrastructure.Identity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueSubscription> VenueSubscriptions => Set<VenueSubscription>();
    public DbSet<WorkShift> WorkShifts => Set<WorkShift>();

    public DbSet<AuthToken> AuthTokens => Set<AuthToken>();
    public DbSet<BilliardTable> BilliardTables => Set<BilliardTable>();
    public DbSet<IoTDevice> IoTDevices => Set<IoTDevice>();
    public DbSet<BilliardSession> BilliardSessions => Set<BilliardSession>();
    public DbSet<AiProviderSettings> AiProviderSettings => Set<AiProviderSettings>();
    public DbSet<AiAnalysisHistory> AiAnalysisHistory => Set<AiAnalysisHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly()
        );
    }
}
