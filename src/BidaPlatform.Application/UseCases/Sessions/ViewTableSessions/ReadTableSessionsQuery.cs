using MediatR;
using BidaPlatform.Application.Models.Sessions.ViewSessions;

namespace BidaPlatform.Application.UseCases.Sessions.ViewTableSessions;

public record ReadTableSessionsQuery(Guid ActorUserId, Guid VenueId, Guid TableId) : IRequest<IEnumerable<SessionResponse>>;
