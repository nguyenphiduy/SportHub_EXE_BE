using MediatR;
using BidaPlatform.Application.Models.Sessions.ViewSessions;

namespace BidaPlatform.Application.UseCases.Sessions.ViewSessions;

public record ReadAllSessionsQuery(Guid ActorUserId, Guid VenueId) : IRequest<IEnumerable<SessionResponse>>;
