using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BidaPlatform.Presentation.Extensions;

/// <summary>
/// Swagger configuration with JWT support
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger with JWT Bearer authentication.
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwt(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BidaPlatform API",
                Version = "v1",
                Description = "BidaPlatform Backend API"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your JWT token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
