using RentIt.Modules.Identity.Application;
using RentIt.Modules.Identity.Infrastructure;
using RentIt.Shared.Abstractions.BackgroundJobs;

using RentIt.Shared.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddControllers();

var assembliesPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
if (assembliesPath != null)
{
    foreach (var file in System.IO.Directory.GetFiles(assembliesPath, "RentIt.Modules.*.Api.dll"))
    {
        var assembly = System.Reflection.Assembly.LoadFrom(file);
        mvcBuilder.AddApplicationPart(assembly);
    }
}
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddSharedEmailServices();
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "RentIt Modular Monolith Host is running.");

app.Run();
