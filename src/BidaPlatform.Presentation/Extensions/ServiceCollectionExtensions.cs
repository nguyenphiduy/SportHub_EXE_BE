using BidaPlatform.Application;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Services;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using BidaPlatform.Infrastructure.Jobs;
using BidaPlatform.Infrastructure.Repositories;
using BidaPlatform.Infrastructure.Security;
using BidaPlatform.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;

namespace BidaPlatform.Presentation.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Testing") ?? true)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(
                        config.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("BidaPlatform.Infrastructure")
                    ));
            }

            var encryptionKey = config["Security:EncryptionKey"];
            if (string.IsNullOrWhiteSpace(encryptionKey))
                throw new InvalidOperationException("Security:EncryptionKey is not configured.");

            EncryptionHelper.ConfigureKey(encryptionKey);

            services.Configure<JwtSettings>(
                config.GetSection("Jwt"));

            var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("Jwt settings are not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                        ),
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = "role"
                    };
                });

            services.AddAuthorization();
            services.AddHttpContextAccessor();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IVenueSubscriptionRepository, VenueSubscriptionRepository>();
            services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();

            services.AddScoped<IBilliardTableRepository, BilliardTableRepository>();
            services.AddScoped<IIoTDeviceRepository, IoTDeviceRepository>();
            services.AddScoped<IBilliardSessionRepository, BilliardSessionRepository>();
            services.AddScoped<IIoTControlService, IoTControlService>();
            services.AddScoped<IRevenueService, RevenueService>();
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();
            services.AddScoped<IVenueAccessChecker, VenueAccessChecker>();
            services.AddScoped<ISubscriptionAccessService, SubscriptionAccessService>();
            services.AddScoped<IAiInsightService, AiInsightService>();
            services.AddScoped<IAiProviderSettingsRepository, AiProviderSettingsRepository>();
            services.AddScoped<IAiAnalysisHistoryRepository, AiAnalysisHistoryRepository>();
            services.AddScoped<IAiProviderSettingsService, AiProviderSettingsService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddHttpClient("IoTClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(5);
            });
            services.AddHttpClient<OpenRouterAiClient>("OpenRouterClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddHostedService<AuthTokenCleanupJob>();
            services.AddHostedService<IoTHealthCheckJob>();
            services.AddHostedService<SubscriptionExpirationJob>();

            return services;
        }
    }
}
