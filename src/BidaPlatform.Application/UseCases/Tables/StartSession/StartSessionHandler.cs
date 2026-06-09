using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.StartSession;

public class StartSessionHandler : IRequestHandler<StartSessionCommand>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IBilliardSessionRepository _sessionRepo;
    private readonly IIoTControlService _iotService;
    private readonly ITableNotifier _tableNotifier;

    public StartSessionHandler(
        IBilliardTableRepository tableRepo,
        IBilliardSessionRepository sessionRepo,
        IIoTControlService iotService,
        ITableNotifier tableNotifier)
    {
        _tableRepo = tableRepo;
        _sessionRepo = sessionRepo;
        _iotService = iotService;
        _tableNotifier = tableNotifier;
    }

    public async Task Handle(StartSessionCommand request, CancellationToken ct)
    {
        var table = await _tableRepo.GetByIdWithDeviceAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        if (!table.IsActive)
            throw new InvalidOperationException("Bàn này không còn hoạt động.");

        if (table.Status == BilliardTableStatus.Playing)
            throw new InvalidOperationException("Bàn này đang có người chơi.");

        if (table.Status == BilliardTableStatus.Maintenance)
            throw new InvalidOperationException("Bàn này đang bảo trì.");

        if (table.IoTDevice != null && table.IoTDevice.IsOnline)
            await _iotService.TurnOnAsync(table.IoTDevice.IpAddress, ct);

        var session = new BilliardSession
        {
            Id = Guid.NewGuid(),
            VenueId = table.VenueId,
            TableId = table.Id,
            StartedByUserId = request.UserId,
            StartTime = DateTime.UtcNow,
            Status = BilliardSessionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _sessionRepo.AddAsync(session, ct);

        table.Status = BilliardTableStatus.Playing;
        table.UpdatedAt = DateTime.UtcNow;
        _tableRepo.Update(table);

        await _sessionRepo.SaveChangesAsync(ct);

        await _tableNotifier.NotifyTableStatusChangedAsync(table.VenueId, table.Id, table.Name, table.Status.ToString(), ct);
    }
}
