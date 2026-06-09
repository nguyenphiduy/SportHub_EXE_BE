using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Dashboard;

namespace BidaPlatform.Application.UseCases.Dashboard.GetDashboardSummary;

public record GetDashboardSummaryQuery(Guid ActorUserId, string ActorRole, Guid? VenueId) : IRequest<DashboardSummaryResponse>;

public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryResponse>
{
    private readonly IDashboardService _dashboardService;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public GetDashboardSummaryHandler(IDashboardService dashboardService, IVenueAccessChecker venueAccessChecker)
    {
        _dashboardService = dashboardService;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task<DashboardSummaryResponse> Handle(GetDashboardSummaryQuery request, CancellationToken ct)
    {
        if (request.ActorRole == "SuperAdmin")
            return await _dashboardService.GetSystemDashboardAsync(ct);

        if (!request.VenueId.HasValue)
            throw new UnauthorizedAccessException("VenueId là bắt buộc cho dashboard theo quán");

        await _venueAccessChecker.EnsureCanAccessVenueAsync(request.ActorUserId, request.ActorRole, request.VenueId, request.VenueId.Value, ct);
        return await _dashboardService.GetVenueDashboardAsync(request.VenueId.Value, ct);
    }
}
