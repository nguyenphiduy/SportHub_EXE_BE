using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Infrastructure.Identity;
using BidaPlatform.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BidaPlatform.Infrastructure.Seed;

public static class DbSeeder
{
    private const string RevenueSeedNotePrefix = "Seed revenue bulk";
    private const int TargetRevenueSeedCount = 24;

    private static readonly string[] LegacyEcommerceTables =
    [
        "cart_item",
        "cart",
        "category",
        "order_item",
        "orders",
        "payment",
        "product_image",
        "product",
        "review"
    ];

    private static readonly string[] LegacyChatTables =
    [
        "message",
        "conversation"
    ];

    public static async Task SeedAsync(
        AppDbContext db,
        ILogger logger)
    {
        await db.Database.MigrateAsync();

        await DropLegacyEcommerceTablesAsync(db, logger);
        await DropLegacyChatTablesAsync(db, logger);

        await SeedSuperAdminUserAsync(db, logger);
        await SeedDemoVenueAsync(db, logger);
        await SeedBilliardTablesAsync(db, logger);
        await SeedIoTDevicesAsync(db, logger);
        await SeedSampleSessionsAsync(db, logger);
        await SeedRevenueSessionsAsync(db, logger);
    }

    private static async Task DropLegacyEcommerceTablesAsync(
        AppDbContext db,
        ILogger logger)
    {
        foreach (var table in LegacyEcommerceTables)
        {
            var sql = $"DROP TABLE IF EXISTS \"{table}\" CASCADE;";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
        logger.LogInformation("Legacy e-commerce tables cleanup completed.");
    }

    private static async Task DropLegacyChatTablesAsync(
        AppDbContext db,
        ILogger logger)
    {
        foreach (var table in LegacyChatTables)
        {
            var sql = $"DROP TABLE IF EXISTS \"{table}\" CASCADE;";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
        logger.LogInformation("Legacy chat tables cleanup completed.");
    }

    private static async Task SeedSuperAdminUserAsync(
        AppDbContext db,
        ILogger logger)
    {
        const string emailPlain = "superadmin@system.com";
        const string passwordPlain = "SuperAdmin987123@";

        var encryptedEmail = EncryptionHelper.EncryptDeterministic(emailPlain);
        var exists = await db.Users.AnyAsync(u => u.Email == encryptedEmail);

        if (exists)
        {
            logger.LogInformation("SuperAdmin account already exists. Skipping seed.");
            return;
        }

        logger.LogInformation("Seeding default SuperAdmin account...");

        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            Email = encryptedEmail,
            Password = BCrypt.Net.BCrypt.HashPassword("Abcd@123"),
            FullName = EncryptionHelper.Encrypt("System Administrator"),
            Role = UserRole.SuperAdmin.ToString(),
            VenueId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.Add(superAdmin);
        await db.SaveChangesAsync();

        logger.LogInformation("SuperAdmin account created successfully.");
    }

    private static async Task SeedDemoVenueAsync(
        AppDbContext db,
        ILogger logger)
    {
        if (await db.Venues.AnyAsync())
        {
            logger.LogInformation("Venues already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding demo venue...");

        var superAdmin = await db.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.SuperAdmin.ToString());

        var now = DateTime.UtcNow;
        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            Name = "Bida Hub Sport Demo",
            Address = "123 Duong ABC, Quan 1, TP.HCM",
            Phone = "0909123456",
            OwnerName = "Quan Ly Mot",
            Status = VenueStatus.Approved,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Venues.Add(venue);
        await db.SaveChangesAsync();

        var managerEmail = EncryptionHelper.EncryptDeterministic("manager1@hubsport.com");
        var managerExists = await db.Users.AnyAsync(u => u.Email == managerEmail);

        User manager;
        if (managerExists)
        {
            manager = await db.Users.FirstAsync(u => u.Email == managerEmail);
            manager.VenueId = venue.Id;
            manager.UpdatedAt = now;
        }
        else
        {
            manager = new User
            {
                Id = Guid.NewGuid(),
                Email = managerEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("Abcd@123"),
                FullName = EncryptionHelper.Encrypt("Quan Ly Mot"),
                Role = UserRole.Manager.ToString(),
                VenueId = venue.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(manager);
        }


        var staffEmail = EncryptionHelper.EncryptDeterministic("staff1@hubsport.com");
        var staffExists = await db.Users.AnyAsync(u => u.Email == staffEmail);

        User staff1;
        if (staffExists)
        {
            staff1 = await db.Users.FirstAsync(u => u.Email == staffEmail);
            staff1.VenueId = venue.Id;
            staff1.UpdatedAt = now;
        }
        else
        {
            staff1 = new User
            {
                Id = Guid.NewGuid(),
                Email = staffEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("Abcd@123"),
                FullName = EncryptionHelper.Encrypt("Nhan Vien Mot"),
                Role = UserRole.Staff.ToString(),
                VenueId = venue.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(staff1);
        }

        var staff2Email = EncryptionHelper.EncryptDeterministic("staff2@hubsport.com");
        var staff2Exists = await db.Users.AnyAsync(u => u.Email == staff2Email);

        User staff2;
        if (staff2Exists)
        {
            staff2 = await db.Users.FirstAsync(u => u.Email == staff2Email);
            staff2.VenueId = venue.Id;
            staff2.UpdatedAt = now;
        }
        else
        {
            staff2 = new User
            {
                Id = Guid.NewGuid(),
                Email = staff2Email,
                Password = BCrypt.Net.BCrypt.HashPassword("Abcd@123"),
                FullName = EncryptionHelper.Encrypt("Nhan Vien Hai"),
                Role = UserRole.Staff.ToString(),
                VenueId = venue.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(staff2);
        }

        var subscription = new VenueSubscription
        {
            Id = Guid.NewGuid(),
            VenueId = venue.Id,
            Plan = SubscriptionPlan.Premium,
            Status = VenueSubscriptionStatus.Active,
            StartDate = now.AddMonths(-1),
            EndDate = now.AddMonths(11),
            AutoRenew = true,
            ApprovedBySuperAdminId = superAdmin?.Id,
            ApprovedAt = now.AddMonths(-1),
            CreatedAt = now.AddMonths(-1),
            UpdatedAt = now
        };
        db.VenueSubscriptions.Add(subscription);

        await db.SaveChangesAsync();

        venue.PrimaryManagerId = manager.Id;
        venue.UpdatedAt = DateTime.UtcNow;
        db.Venues.Update(venue);
        await db.SaveChangesAsync();

        logger.LogInformation(
            "Seeded demo venue '{Name}' with Manager and Staff accounts.",
            venue.Name);
    }

    private static async Task SeedBilliardTablesAsync(AppDbContext db, ILogger logger)
    {
        var venue = await db.Venues.FirstOrDefaultAsync();
        if (venue == null)
        {
            logger.LogInformation("No venue found. Skip billiard tables seed.");
            return;
        }

        if (await db.BilliardTables.AnyAsync(x => x.VenueId == venue.Id))
        {
            logger.LogInformation("Billiard tables already seeded for venue. Skipping.");
            return;
        }

        logger.LogInformation("Seeding billiard tables...");

        var tables = new[]
        {
            (Name: "Ban 1",  Type: BilliardTableType.Standard, Price: 100_000m),
            (Name: "Ban 2",  Type: BilliardTableType.Standard, Price: 100_000m),
            (Name: "Ban 3",  Type: BilliardTableType.Standard, Price: 100_000m),
            (Name: "Ban 4",  Type: BilliardTableType.Standard, Price: 100_000m),
            (Name: "Ban 5",  Type: BilliardTableType.VIP,      Price: 130_000m),
            (Name: "Ban 6",  Type: BilliardTableType.VIP,      Price: 130_000m),
            (Name: "Ban 7",  Type: BilliardTableType.Premium,  Price: 180_000m),
            (Name: "Ban 8",  Type: BilliardTableType.Premium,  Price: 180_000m),
            (Name: "Ban 9",  Type: BilliardTableType.Standard, Price: 100_000m),
            (Name: "Ban 10", Type: BilliardTableType.Standard, Price: 100_000m),
        };

        var now = DateTime.UtcNow;
        foreach (var t in tables)
        {
            db.BilliardTables.Add(new BilliardTable
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                Name = t.Name,
                Type = t.Type,
                PricePerHour = t.Price,
                Status = BilliardTableStatus.Available,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} billiard tables.", tables.Length);
    }

    private static async Task SeedIoTDevicesAsync(
        AppDbContext db,
        ILogger logger)
    {
        var tables = await db.BilliardTables
            .OrderBy(x => x.Name)
            .ToListAsync();

        if (tables.Count == 0)
        {
            logger.LogInformation("No billiard tables found. Skip IoT device seed.");
            return;
        }

        var existingDevices = await db.IoTDevices
            .ToDictionaryAsync(x => x.TableId);

        var createdCount = 0;
        var normalizedOnlineCount = 0;
        var now = DateTime.UtcNow;

        for (var i = 0; i < tables.Count; i++)
        {
            var table = tables[i];
            if (existingDevices.TryGetValue(table.Id, out var existingDevice))
            {
                if (!existingDevice.IsOnline)
                {
                    existingDevice.IsOnline = true;
                    existingDevice.UpdatedAt = now;
                    normalizedOnlineCount++;
                }
                continue;
            }

            db.IoTDevices.Add(new IoTDevice
            {
                Id = Guid.NewGuid(),
                VenueId = table.VenueId,
                TableId = table.Id,
                IpAddress = $"192.168.1.{101 + i}",
                DeviceName = $"ESP-{table.Name}",
                IsOnline = true,
                CreatedAt = now,
                UpdatedAt = now
            });

            createdCount++;
        }

        if (createdCount == 0 && normalizedOnlineCount == 0)
        {
            logger.LogInformation("IoT devices already seeded. Skipping.");
            return;
        }

        await db.SaveChangesAsync();
        logger.LogInformation(
            "Seeded {CreatedCount} IoT devices and normalized {NormalizedCount} devices to online.",
            createdCount,
            normalizedOnlineCount);
    }

    private static async Task SeedSampleSessionsAsync(
        AppDbContext db,
        ILogger logger)
    {
        var venue = await db.Venues.FirstOrDefaultAsync();
        if (venue == null)
        {
            logger.LogInformation("No venue found. Skip session seed.");
            return;
        }

        if (await db.BilliardSessions.AnyAsync())
        {
            logger.LogInformation("Billiard sessions already seeded. Skipping.");
            return;
        }

        var starterUser = await db.Users
            .Where(x => x.VenueId == venue.Id)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (starterUser == null)
        {
            logger.LogInformation("No user found in venue. Skip session seed.");
            return;
        }

        var tables = await db.BilliardTables
            .Where(x => x.VenueId == venue.Id)
            .OrderBy(x => x.Name)
            .Take(4)
            .ToListAsync();

        if (tables.Count == 0)
        {
            logger.LogInformation("No billiard tables found. Skip session seed.");
            return;
        }

        var now = DateTime.UtcNow;
        var sessions = new List<BilliardSession>();

        if (tables.Count >= 1)
        {
            var t1 = tables[0];
            t1.Status = BilliardTableStatus.Playing;
            t1.UpdatedAt = now;
            db.BilliardTables.Update(t1);

            sessions.Add(new BilliardSession
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                TableId = t1.Id,
                StartedByUserId = starterUser.Id,
                StartTime = now.AddMinutes(-45),
                EndTime = null,
                DurationMinutes = null,
                TotalPrice = null,
                Status = BilliardSessionStatus.Active,
                Note = "Sample active session",
                PaymentMethod = null,
                PaymentStatus = BilliardPaymentStatus.Pending,
                CreatedAt = now.AddMinutes(-45)
            });
        }

        if (tables.Count >= 2)
        {
            var t2 = tables[1];
            sessions.Add(CreateCompletedSession(venue.Id, t2, starterUser.Id, now.AddHours(-4), 90));
        }

        if (tables.Count >= 3)
        {
            var t3 = tables[2];
            sessions.Add(CreateCompletedSession(venue.Id, t3, starterUser.Id, now.AddHours(-8), 120));
        }

        if (tables.Count >= 4)
        {
            var t4 = tables[3];
            sessions.Add(CreateCompletedSession(venue.Id, t4, starterUser.Id, now.AddDays(-1), 60));
        }

        db.BilliardSessions.AddRange(sessions);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} sample billiard sessions.", sessions.Count);
    }

    private static BilliardSession CreateCompletedSession(
        Guid venueId,
        BilliardTable table,
        Guid startedByUserId,
        DateTime startTime,
        int durationMinutes)
    {
        var endTime = startTime.AddMinutes(durationMinutes);
        var totalPrice = Math.Round(table.PricePerHour * durationMinutes / 60m, 2);

        return new BilliardSession
        {
            Id = Guid.NewGuid(),
            VenueId = venueId,
            TableId = table.Id,
            StartedByUserId = startedByUserId,
            StartTime = startTime,
            EndTime = endTime,
            DurationMinutes = durationMinutes,
            TotalPrice = totalPrice,
            Status = BilliardSessionStatus.Completed,
            Note = "Sample completed session",
            PaymentMethod = BilliardPaymentMethod.Cash,
            PaymentStatus = BilliardPaymentStatus.Paid,
            CreatedAt = startTime
        };
    }

    private static async Task SeedRevenueSessionsAsync(
        AppDbContext db,
        ILogger logger)
    {
        var venue = await db.Venues.FirstOrDefaultAsync();
        if (venue == null)
        {
            logger.LogInformation("No venue found. Skip revenue seed.");
            return;
        }

        var existingSeedCount = await db.BilliardSessions
            .CountAsync(x => x.Note != null && x.Note.StartsWith(RevenueSeedNotePrefix));

        if (existingSeedCount >= TargetRevenueSeedCount)
        {
            logger.LogInformation("Revenue sessions already seeded with {Count} records.", existingSeedCount);
            return;
        }

        var tables = await db.BilliardTables
            .Where(x => x.VenueId == venue.Id && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        var users = await db.Users
            .Where(x => x.VenueId == venue.Id && x.IsActive)
            .ToListAsync();

        if (tables.Count == 0 || users.Count == 0)
        {
            logger.LogInformation("Cannot seed revenue sessions: missing active tables or users.");
            return;
        }

        var needToCreate = TargetRevenueSeedCount - existingSeedCount;
        var now = DateTime.UtcNow;
        var startWindow = now.Date.AddDays(-90);
        var durations = new[] { 45, 60, 75, 90, 120, 150 };
        var sessions = new List<BilliardSession>(needToCreate);

        for (var i = 0; i < needToCreate; i++)
        {
            var sequence = existingSeedCount + i;
            var table = tables[sequence % tables.Count];
            var user = users[(sequence * 7 + 3) % users.Count];

            var dayOffset = (sequence * 13 + 5) % 90;
            var hour = 8 + ((sequence * 5) % 15);
            var minute = (sequence * 17) % 60;
            var startTime = startWindow.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);

            var duration = durations[sequence % durations.Length];
            var endTime = startTime.AddMinutes(duration);
            var totalPrice = Math.Ceiling(duration / 60m * table.PricePerHour);

            sessions.Add(new BilliardSession
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                TableId = table.Id,
                StartedByUserId = user.Id,
                StartTime = startTime,
                EndTime = endTime,
                DurationMinutes = duration,
                TotalPrice = totalPrice,
                Status = BilliardSessionStatus.Completed,
                Note = $"{RevenueSeedNotePrefix} #{sequence + 1}",
                PaymentMethod = sequence % 3 == 0
                    ? BilliardPaymentMethod.BankTransfer
                    : BilliardPaymentMethod.Cash,
                PaymentStatus = BilliardPaymentStatus.Paid,
                CreatedAt = startTime
            });
        }

        db.BilliardSessions.AddRange(sessions);
        await db.SaveChangesAsync();
        logger.LogInformation(
            "Seeded {Count} additional revenue sessions.",
            sessions.Count);
    }
}
