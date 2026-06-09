using MediatR;

namespace BidaPlatform.Application.UseCases.Tables.DeleteTable;

public record DeleteTableCommand(Guid ActorUserId, Guid VenueId, Guid TableId) : IRequest;
