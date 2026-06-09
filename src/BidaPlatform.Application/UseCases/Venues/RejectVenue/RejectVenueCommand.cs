using MediatR;
using BidaPlatform.Application.Models.Venues.Common;

namespace BidaPlatform.Application.UseCases.Venues.RejectVenue;

public record RejectVenueCommand(Guid VenueId) : IRequest<VenueResponse>;
