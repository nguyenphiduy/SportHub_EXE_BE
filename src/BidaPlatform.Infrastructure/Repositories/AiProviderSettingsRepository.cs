using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class AiProviderSettingsRepository : IAiProviderSettingsRepository
{
    private readonly AppDbContext _db;

    public AiProviderSettingsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AiProviderSettings?> GetAsync(CancellationToken ct = default)
        => await _db.AiProviderSettings.FirstOrDefaultAsync(ct);

    public async Task AddAsync(AiProviderSettings settings, CancellationToken ct = default)
        => await _db.AiProviderSettings.AddAsync(settings, ct);

    public void Update(AiProviderSettings settings)
        => _db.AiProviderSettings.Update(settings);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
