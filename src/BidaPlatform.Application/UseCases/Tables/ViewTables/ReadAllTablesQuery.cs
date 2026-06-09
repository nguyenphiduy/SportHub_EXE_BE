using MediatR;
using BidaPlatform.Application.Models.Tables.ViewTables;

namespace BidaPlatform.Application.UseCases.Tables.ViewTables;

public record ReadAllTablesQuery(Guid ActorUserId, Guid VenueId, bool ActiveOnly = false) : IRequest<IEnumerable<TableResponse>>;
