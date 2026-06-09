using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BidaPlatform.Infrastructure.Identity;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../BidaPlatform.Presentation"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Secrets.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var rawConnection = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(rawConnection) || rawConnection == "__DB_CONNECTION__")
        {
            rawConnection = "Host=localhost;Port=5432;Database=HubSport;Username=postgres;Password=postgres";
        }

        var builder = new NpgsqlConnectionStringBuilder(rawConnection)
        {
            Database = "HubSport"
        };

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(builder.ConnectionString, b => b.MigrationsAssembly("BidaPlatform.Infrastructure"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
