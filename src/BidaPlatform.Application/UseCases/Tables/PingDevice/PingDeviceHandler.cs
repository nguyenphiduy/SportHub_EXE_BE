using MediatR;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.PingDevice;

public class PingDeviceHandler : IRequestHandler<PingDeviceCommand, PingDeviceResult>
{
    private readonly IIoTDeviceRepository _deviceRepo;
    private readonly IIoTControlService   _iotService;

    public PingDeviceHandler(IIoTDeviceRepository deviceRepo, IIoTControlService iotService)
    {
        _deviceRepo = deviceRepo;
        _iotService  = iotService;
    }

    public async Task<PingDeviceResult> Handle(PingDeviceCommand request, CancellationToken ct)
    {
        var device = await _deviceRepo.GetByTableIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Bàn {request.TableId} chưa có thiết bị IoT.");

        var isOnline = await _iotService.PingAsync(device.IpAddress, ct);

        // Cập nhật trạng thái thực tế vào DB
        device.IsOnline  = isOnline;
        device.UpdatedAt = DateTime.UtcNow;
        _deviceRepo.Update(device);
        await _deviceRepo.SaveChangesAsync(ct);

        return new PingDeviceResult(isOnline, device.IpAddress);
    }
}
