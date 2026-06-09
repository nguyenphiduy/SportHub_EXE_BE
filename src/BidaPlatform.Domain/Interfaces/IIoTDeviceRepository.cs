using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IIoTDeviceRepository
{
    Task<List<IoTDevice>> GetAllAsync(CancellationToken ct = default);
    Task<IoTDevice?> GetByTableIdAsync(Guid tableId, CancellationToken ct = default);
    Task<IoTDevice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(IoTDevice device, CancellationToken ct = default);
    void Update(IoTDevice device);
    void Remove(IoTDevice device);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
