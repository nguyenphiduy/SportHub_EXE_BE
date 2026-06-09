using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class IoTDeviceRepository : IIoTDeviceRepository
{
    private readonly AppDbContext _db;

    public IoTDeviceRepository(AppDbContext db) => _db = db;

    public async Task<List<IoTDevice>> GetAllAsync(CancellationToken ct = default)
        => await _db.IoTDevices.ToListAsync(ct);

    public async Task<IoTDevice?> GetByTableIdAsync(Guid tableId, CancellationToken ct = default)
        => await _db.IoTDevices.FirstOrDefaultAsync(x => x.TableId == tableId, ct);

    public async Task<IoTDevice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.IoTDevices.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(IoTDevice device, CancellationToken ct = default)
        => await _db.IoTDevices.AddAsync(device, ct);

    public void Update(IoTDevice device)
        => _db.IoTDevices.Update(device);

    public void Remove(IoTDevice device)
        => _db.IoTDevices.Remove(device);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
