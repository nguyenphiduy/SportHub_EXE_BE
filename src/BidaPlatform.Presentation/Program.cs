using BidaPlatform.Application;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using BidaPlatform.Infrastructure.Seed;
using BidaPlatform.Presentation.Extensions;
using BidaPlatform.Presentation.Hubs;
using BidaPlatform.Presentation.Middlewares;
using BidaPlatform.Presentation.SignalR;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONFIGURATION – LOAD SECRETS SAFELY
// =====================================================
builder.Configuration
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

// Map Render flat environment variables to ASP.NET Core structured configuration keys
var renderConfig = new Dictionary<string, string?>();

var dbConn = Environment.GetEnvironmentVariable("DB_CONNECTION") 
             ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (!string.IsNullOrEmpty(dbConn))
    renderConfig["ConnectionStrings:DefaultConnection"] = dbConn;

var encKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
             ?? Environment.GetEnvironmentVariable("Security__EncryptionKey");
if (!string.IsNullOrEmpty(encKey))
    renderConfig["Security:EncryptionKey"] = encKey;

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? Environment.GetEnvironmentVariable("Jwt__SecretKey");
if (!string.IsNullOrEmpty(jwtSecret))
    renderConfig["Jwt:SecretKey"] = jwtSecret;

var emailUser = Environment.GetEnvironmentVariable("EMAIL_USERNAME")
                ?? Environment.GetEnvironmentVariable("Email__Username");
if (!string.IsNullOrEmpty(emailUser))
    renderConfig["Email:Username"] = emailUser;

var emailPass = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
                ?? Environment.GetEnvironmentVariable("Email__Password");
if (!string.IsNullOrEmpty(emailPass))
    renderConfig["Email:Password"] = emailPass;

var iotEnabled = Environment.GetEnvironmentVariable("IOT_ENABLED")
                 ?? Environment.GetEnvironmentVariable("IoT__Enabled");
if (!string.IsNullOrEmpty(iotEnabled))
    renderConfig["IoT:Enabled"] = iotEnabled;

if (renderConfig.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(renderConfig);
}

// =====================================================
// REGISTER SERVICES
// =====================================================
var dbConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (dbConnectionStr != null)
{
    var maskedCs = System.Text.RegularExpressions.Regex.Replace(dbConnectionStr, @"(?i)Password=[^;]+", "Password=******");
    Console.WriteLine($"[Startup] Resolved ConnectionString: {maskedCs}");
}
else
{
    Console.WriteLine("[Startup] Resolved ConnectionString is NULL");
}

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
builder.Services.AddScoped<INotificationBroadcaster, NotificationBroadcaster>();
builder.Services.AddScoped<ITableNotifier, TableNotifier>();

// =====================================================
// MEDIATR + FLUENT VALIDATION
// =====================================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(BidaPlatform.Application.Behaviors.ValidationBehavior<,>)
);

// =====================================================
// CORS (ONLY ONE – SIGNALR COMPATIBLE)
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =====================================================
// CONTROLLERS
// =====================================================
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// =====================================================
// SWAGGER
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BidaPlatform API",
        Version = "v1",
        Description = "BidaPlatform Backend API"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter 'Bearer {token}'",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// =====================================================
// AUTO MIGRATION + SEED
// =====================================================
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DbSeeder");

    db.Database.Migrate();
    await DbSeeder.SeedAsync(db, logger);
}
// =====================================================
// PIPELINE (ORDER IS CRITICAL)
// =====================================================

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BidaPlatform API v1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<DisableEcommerceMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<VenueStatusMiddleware>();

app.MapControllers();
app.MapHub<TableHub>("/hubs/tables");

app.Run();

public partial class Program { }
