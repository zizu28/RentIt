using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using RentIt.Client.Web;
using RentIt.Client.Web.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Authentication & BFF setup
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CookieHandler>();

// Configure HttpClient to talk to the BFF via the CookieHandler
builder.Services.AddHttpClient("BFF", client =>
{
    // The BFF URL
    client.BaseAddress = new Uri("https://localhost:7046");
})
.AddHttpMessageHandler<CookieHandler>();

// Make the BFF client the default one
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BFF"));

// Register custom authentication state provider and auth service
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (BffAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Mock UI Services
builder.Services.AddScoped<RentIt.Client.Web.Services.IPropertyService, RentIt.Client.Web.Services.PropertyService>();
builder.Services.AddScoped<RentIt.Client.Web.Services.IBookingService, RentIt.Client.Web.Services.MockBookingService>();
await builder.Build().RunAsync();
