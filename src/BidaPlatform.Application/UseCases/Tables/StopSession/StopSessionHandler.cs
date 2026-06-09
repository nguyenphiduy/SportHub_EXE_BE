using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Sessions.ViewSessions;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.StopSession;

public class StopSessionHandler : IRequestHandler<StopSessionCommand, SessionResponse>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IBilliardSessionRepository _sessionRepo;
    private readonly IIoTControlService _iotService;
    private readonly ITableNotifier _tableNotifier;

    public StopSessionHandler(
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

    public async Task<SessionResponse> Handle(StopSessionCommand request, CancellationToken ct)
    {
        var table = await _tableRepo.GetByIdWithDeviceAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        if (table.VenueId != request.VenueId)
            throw new UnauthorizedAccessException("Không thể kết thúc session cho bàn của quán khác");

        if (table.Status != BilliardTableStatus.Playing)
            throw new InvalidOperationException("Bàn này không đang trong trạng thái chơi.");

        var session = await _sessionRepo.GetActiveSessionByTableIdAsync(request.TableId, ct)
            ?? throw new InvalidOperationException("Không tìm thấy session đang hoạt động cho bàn này.");

        var endTime = DateTime.UtcNow;
        var durationMinutes = (int)Math.Ceiling((endTime - session.StartTime).TotalMinutes);
        var totalPrice = Math.Ceiling(durationMinutes / 60m * table.PricePerHour);

        session.EndTime = endTime;
        session.DurationMinutes = durationMinutes;
        session.TotalPrice = totalPrice;
        session.Status = BilliardSessionStatus.Completed;
        if (!string.IsNullOrWhiteSpace(request.Note))
            session.Note = request.Note.Trim();

        if (request.PaymentMethod.HasValue)
        {
            session.PaymentMethod = request.PaymentMethod.Value;
            session.PaymentStatus = request.PaymentMethod.Value == BilliardPaymentMethod.Cash
                ? BilliardPaymentStatus.Paid
                : BilliardPaymentStatus.Pending;
        }

        _sessionRepo.Update(session);

        if (table.IoTDevice != null && table.IoTDevice.IsOnline)
            await _iotService.TurnOffAsync(table.IoTDevice.IpAddress, ct);

        table.Status = BilliardTableStatus.Available;
        table.UpdatedAt = DateTime.UtcNow;
        _tableRepo.Update(table);

        await _sessionRepo.SaveChangesAsync(ct);

        await _tableNotifier.NotifyTableStatusChangedAsync(table.VenueId, table.Id, table.Name, table.Status.ToString(), ct);

        return new SessionResponse
        {
            Id = session.Id,
            TableId = table.Id,
            TableName = table.Name,
            StartedByUserName = string.Empty,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            DurationMinutes = session.DurationMinutes,
            TotalPrice = session.TotalPrice,
            Status = session.Status.ToString(),
            Note = session.Note,
            PaymentMethod = session.PaymentMethod?.ToString(),
            PaymentStatus = session.PaymentStatus?.ToString()
        };
    }
}
