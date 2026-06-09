using MediatR;
using BidaPlatform.Application.Models.Tables.UpdateTable;

namespace BidaPlatform.Application.UseCases.Tables.UpdateTable;

public record UpdateTableCommand(Guid ActorUserId, Guid VenueId, Guid TableId, UpdateTableRequest Request) : IRequest;
