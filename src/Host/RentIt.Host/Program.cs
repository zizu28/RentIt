using System.Reflection;
using RentIt.Modules.Identity.Application;
using RentIt.Modules.Identity.Infrastructure;
using RentIt.Shared.Abstractions.BackgroundJobs;

using RentIt.Shared.Infrastructure.Email;
using RentIt.Shared.Infrastructure.Logging;
using RentIt.Shared.Infrastructure.Pdf;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedLogging();

var mvcBuilder = builder.Services.AddControllers();

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
builder.Services.AddSharedPdfServices();
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.AddHangfireDashBoard();
app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "RentIt Modular Monolith Host is running.");

app.Run();
