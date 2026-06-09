using MediatR;

namespace BidaPlatform.Application.UseCases.Tables.StartSession;

public record StartSessionCommand(Guid TableId, Guid UserId) : IRequest;
