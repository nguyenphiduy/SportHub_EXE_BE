using MediatR;
using BidaPlatform.Application.Models.Sessions.ViewSessions;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.UseCases.Tables.StopSession;

public record StopSessionCommand(Guid ActorUserId, Guid VenueId, Guid TableId, string? Note = null, BilliardPaymentMethod? PaymentMethod = null) : IRequest<SessionResponse>;
