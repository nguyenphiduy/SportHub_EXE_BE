using System.Security.Claims;
using BidaPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BidaPlatform.Infrastructure.Security;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id
        : Guid.Empty;

    public string Role => User?.FindFirstValue("role") ?? string.Empty;

    public Guid? VenueId => Guid.TryParse(User?.FindFirstValue("venueId"), out var id)
        ? id
        : null;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
}
