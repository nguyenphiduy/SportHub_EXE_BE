using System.Net;
using System.Text.Json;

namespace BidaPlatform.Presentation.Middlewares;

public class DisableEcommerceMiddleware
{
    private static readonly string[] DisabledPrefixes =
    [
        "/api/cart",
        "/api/categories",
        "/api/orders",
        "/api/payments",
        "/api/product-images",
        "/api/products",
        "/api/reviews"
    ];

    private readonly RequestDelegate _next;

    public DisableEcommerceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (!string.IsNullOrWhiteSpace(path) &&
            DisabledPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Gone;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                message = "E-commerce features have been disabled. This system now supports only IoT, billiard table operations, and revenue management.",
                code = "ECOMMERCE_DISABLED"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            return;
        }

        await _next(context);
    }
}