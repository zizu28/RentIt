using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using RentIt.Modules.Identity.Application;
using RentIt.Modules.Identity.Infrastructure;
using RentIt.Modules.Properties.Application;
using RentIt.Modules.Properties.Infrastructure;
using RentIt.Modules.Bookings.Api;
using RentIt.Modules.Payments.Application;
using RentIt.Modules.Payments.Infrastructure;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Infrastructure.Email;
using RentIt.Shared.Infrastructure.Messaging;
using RentIt.Shared.Infrastructure.Logging;
using RentIt.Shared.Infrastructure.Pdf;
using RentIt.Shared.Infrastructure.Security;
using RentIt.Shared.Infrastructure.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedLogging();

var mvcBuilder = builder.Services.AddControllers();

// Add global exception handling
builder.Services.AddExceptionHandler<RentIt.Shared.Infrastructure.Middleware.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var assembliesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
if (assembliesPath != null)
{
    foreach (var file in Directory.GetFiles(assembliesPath, "RentIt.Modules.*.Api.dll"))
    {
        var assembly = Assembly.LoadFrom(file);
        mvcBuilder.AddApplicationPart(assembly);
    }
}
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddSharedEmailServices();
builder.Services.AddSharedMessaging();
builder.Services.AddSharedSecurity();
builder.Services.AddSharedPdfServices();
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddPropertiesApplication();
builder.Services.AddPropertiesInfrastructure(builder.Configuration);

builder.Services.AddBookingsModule(builder.Configuration);

builder.Services.AddPaymentsApplication();
builder.Services.AddPaymentsInfrastructure(builder.Configuration);
// Add Authentication for the monolith to validate JWTs forwarded by the BFF
var secretKey = builder.Configuration["JWT:Key"] ?? "super_secret_key_that_is_at_least_32_characters_long_for_hmac_sha256!";
var issuer = builder.Configuration["JWT:Issuer"] ?? "RentIt";
var audience = builder.Configuration["JWT:Audience"] ?? "RentIt";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Serilog.Log.Error("Host JWT Authentication Failed: {Exception}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (context.AuthenticateFailure != null)
                {
                    Serilog.Log.Error("Host JWT Challenge Failure: {Failure}", context.AuthenticateFailure.Message);
                }
                else
                {
                    Serilog.Log.Error("Host JWT Challenge triggered without specific Exception. Error: {Error}, ErrorDescription: {ErrorDescription}", context.Error, context.ErrorDescription);
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ─── Rate Limiting (replaces Ocelot Gateway rate limiting) ────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Auth endpoints: 5 requests per 1-minute window per IP (login/register are expensive)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // General API endpoints: 30 requests per 30-second window per IP
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromSeconds(30);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    });

    // Global fallback: 100 requests per minute per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Too many requests. Please slow down." },
            cancellationToken);
    };
});

// ─── Output Caching (reduces DB load on read-heavy GET endpoints) ─────────────
builder.Services.AddOutputCache(options =>
{
    // Default policy: cache GET responses for 60 seconds
    options.AddBasePolicy(builder => builder
        .With(c => c.HttpContext.Request.Method == "GET")
        .Expire(TimeSpan.FromSeconds(60))
        .Tag("default"));

    // Short-lived cache for user profile (10s) — balances freshness vs. performance
    options.AddPolicy("short", builder => builder
        .Expire(TimeSpan.FromSeconds(10))
        .Tag("short"));

    // Longer cache for property listings (5 min) — data changes infrequently
    options.AddPolicy("listings", builder => builder
        .Expire(TimeSpan.FromMinutes(5))
        .Tag("listings"));
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();
app.AddHangfireDashBoard();
RentIt.Modules.Bookings.Infrastructure.BookingsInfrastructureServiceRegistration.ConfigureBookingsJobs();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseOutputCache();

app.MapControllers();

app.MapGet("/", () => "RentIt Modular Monolith Host is running.");

app.Run();

