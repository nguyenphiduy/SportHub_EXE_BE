using MediatR;
using BidaPlatform.Application.Models.Sessions.ViewSessions;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Sessions.ViewSessions;

public class ReadAllSessionsHandler : IRequestHandler<ReadAllSessionsQuery, IEnumerable<SessionResponse>>
{
    private readonly IBilliardSessionRepository _sessionRepo;

    public ReadAllSessionsHandler(IBilliardSessionRepository sessionRepo) => _sessionRepo = sessionRepo;

    public async Task<IEnumerable<SessionResponse>> Handle(ReadAllSessionsQuery request, CancellationToken ct)
    {
        var sessions = await _sessionRepo.GetAllAsync(ct);

        return sessions.Select(s => new SessionResponse
        {
            Id = s.Id,
            TableId = s.TableId,
            TableName = s.Table.Name,
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
