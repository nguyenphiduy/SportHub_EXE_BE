using MediatR;

namespace BidaPlatform.Application.UseCases.Tables.RestoreTable;

public record RestoreTableCommand(Guid ActorUserId, Guid VenueId, Guid TableId) : IRequest;
