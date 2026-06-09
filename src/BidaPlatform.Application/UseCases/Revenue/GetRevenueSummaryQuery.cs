using MediatR;
using BidaPlatform.Application.Models.Revenue;

namespace BidaPlatform.Application.UseCases.Revenue;

public record GetRevenueSummaryQuery(
    Guid ActorUserId,
    Guid? VenueId,
    string Period,
    DateTime? AnchorDate = null
) : IRequest<RevenueSummaryResponse>;
