using System.Security.Claims;
using System.Text.Json;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Presentation.Middlewares;

public class VenueStatusMiddleware
{
    private static readonly string[] ExcludedPaths =
    [
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/refresh",
        "/swagger",
        "/api/venues/register"
    ];

    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public VenueStatusMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // Skip excluded paths
        if (ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // Check if user is Manager
        var role = context.User.FindFirst("role")?.Value;
        var venueIdClaim = context.User.FindFirst("venue_id")?.Value;

        if (role == "Manager" && !string.IsNullOrEmpty(venueIdClaim))
        {
            if (Guid.TryParse(venueIdClaim, out var venueId))
            {
                using var scope = _serviceProvider.CreateScope();
                var venueRepo = scope.ServiceProvider.GetRequiredService<IVenueRepository>();
                var venue = await venueRepo.GetByIdAsync(venueId);

                if (venue != null && venue.Status != VenueStatus.Approved)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";

                    var payload = new
                    {
                        message = "Quán của bạn chưa được phê duyệt. Vui lòng liên hệ Admin để được duyệt.",
                        code = "VENUE_NOT_APPROVED"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                    return;
                }
            }
        }

        await _next(context);
    }
}
