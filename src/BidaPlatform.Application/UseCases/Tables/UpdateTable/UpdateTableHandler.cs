using MediatR;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.UpdateTable;

public class UpdateTableHandler : IRequestHandler<UpdateTableCommand>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IIoTDeviceRepository _deviceRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public UpdateTableHandler(IBilliardTableRepository tableRepo, IIoTDeviceRepository deviceRepo, IVenueAccessChecker venueAccessChecker)
    {
        _tableRepo = tableRepo;
        _deviceRepo = deviceRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task Handle(UpdateTableCommand request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanManageVenueAsync(request.ActorUserId, UserRole.Manager.ToString(), request.VenueId, request.VenueId, ct);

        var table = await _tableRepo.GetByIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        if (table.VenueId != request.VenueId)
            throw new UnauthorizedAccessException("Không thể cập nhật bàn của quán khác");

        var req = request.Request;

        if (!string.IsNullOrWhiteSpace(req.Name))
            table.Name = req.Name;

        if (!string.IsNullOrWhiteSpace(req.Type))
            table.Type = Enum.Parse<BilliardTableType>(req.Type, ignoreCase: true);

        if (req.PricePerHour.HasValue && req.PricePerHour.Value > 0)
            table.PricePerHour = req.PricePerHour.Value;

        table.UpdatedAt = DateTime.UtcNow;
        _tableRepo.Update(table);
        await _tableRepo.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(req.DeviceIpAddress))
        {
            var device = await _deviceRepo.GetByTableIdAsync(table.Id, ct);
            if (device == null)
            {
                device = new IoTDevice
                {
                    Id = Guid.NewGuid(),
                    VenueId = request.VenueId,
                    TableId = table.Id,
                    IpAddress = req.DeviceIpAddress,
                    DeviceName = req.DeviceName,
                    IsOnline = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _deviceRepo.AddAsync(device, ct);
            }
            else
            {
                device.IpAddress = req.DeviceIpAddress;
                if (req.DeviceName != null) device.DeviceName = req.DeviceName;
                device.IsOnline = true;
                device.UpdatedAt = DateTime.UtcNow;
                _deviceRepo.Update(device);
            }
            await _deviceRepo.SaveChangesAsync(ct);
        }
    }
}
