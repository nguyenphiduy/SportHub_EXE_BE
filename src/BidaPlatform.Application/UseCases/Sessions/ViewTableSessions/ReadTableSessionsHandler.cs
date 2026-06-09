using MediatR;
using BidaPlatform.Application.Models.Sessions.ViewSessions;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Sessions.ViewTableSessions;

public class ReadTableSessionsHandler : IRequestHandler<ReadTableSessionsQuery, IEnumerable<SessionResponse>>
{
    private readonly IBilliardSessionRepository _sessionRepo;
    private readonly IBilliardTableRepository _tableRepo;

    public ReadTableSessionsHandler(IBilliardSessionRepository sessionRepo, IBilliardTableRepository tableRepo)
    {
        _sessionRepo = sessionRepo;
        _tableRepo = tableRepo;
    }

    public async Task<IEnumerable<SessionResponse>> Handle(ReadTableSessionsQuery request, CancellationToken ct)
    {
        var table = await _tableRepo.GetByIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        var sessions = await _sessionRepo.GetByTableIdAsync(request.TableId, ct);

        return sessions.Select(s => new SessionResponse
        {
            Id = s.Id,
            TableId = s.TableId,
            TableName = table.Name,
            StartedByUserName = s.StartedByUser.FullName,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            DurationMinutes = s.DurationMinutes,
            TotalPrice = s.TotalPrice,
            Status = s.Status.ToString(),
            Note = s.Note,
            PaymentMethod = s.PaymentMethod?.ToString(),
            PaymentStatus = s.PaymentStatus?.ToString()
        });
    }
}
