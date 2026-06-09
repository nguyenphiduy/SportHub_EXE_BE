using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.CreateTable;

public class CreateTableHandler : IRequestHandler<CreateTableCommand>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IIoTDeviceRepository _deviceRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public CreateTableHandler(IBilliardTableRepository tableRepo, IIoTDeviceRepository deviceRepo, IVenueAccessChecker venueAccessChecker)
    {
        _tableRepo = tableRepo;
        _deviceRepo = deviceRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task Handle(CreateTableCommand request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanManageVenueAsync(request.ActorUserId, UserRole.Manager.ToString(), request.VenueId, request.VenueId, ct);

        var type = Enum.Parse<BilliardTableType>(request.Type, ignoreCase: true);

        var table = new BilliardTable
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            Name = request.Name,
            Type = type,
            PricePerHour = request.PricePerHour,
            Status = BilliardTableStatus.Available,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _tableRepo.AddAsync(table, ct);
        await _tableRepo.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(request.DeviceIpAddress))
        {
            var device = new IoTDevice
            {
                Id = Guid.NewGuid(),
                VenueId = request.VenueId,
                TableId = table.Id,
                IpAddress = request.DeviceIpAddress,
                DeviceName = request.DeviceName,
                IsOnline = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _deviceRepo.AddAsync(device, ct);
            await _deviceRepo.SaveChangesAsync(ct);
        }
    }
}
