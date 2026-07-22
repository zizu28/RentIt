using RentIt.Modules.Identity.Application;
using RentIt.Modules.Identity.Infrastructure;
using RentIt.Shared.Abstractions.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "RentIt Modular Monolith Host is running.");

app.Run();
