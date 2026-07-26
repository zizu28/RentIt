using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri("https://localhost:7262"); 
    client.DefaultRequestHeaders.Add("X-Client-Id", "BFF");
});

var customOAuthEvents = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
{
    OnRedirectToAuthorizationEndpoint = context =>
    {
        if (context.Request.Path.StartsWithSegments("/bff/auth/challenge"))
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new { url = context.RedirectUri });
        }
        
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "RentIt.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    };
})
.AddCookie("ExternalCookie")
.AddGoogle(options =>
{
    options.SignInScheme = "ExternalCookie";
    options.ClientId = configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
    options.Events = customOAuthEvents;
})
.AddFacebook(options =>
{
    options.SignInScheme = "ExternalCookie";
    options.AppId = configuration["Authentication:Facebook:AppId"]!;
    options.AppSecret = configuration["Authentication:Facebook:AppSecret"]!;
    options.AccessDeniedPath = "/AccessDeniedPathInfo";
    options.CallbackPath = "/signin-facebook";
    options.SaveTokens = true;
    options.Events = customOAuthEvents;
})
.AddMicrosoftAccount(options =>
{
    options.SignInScheme = "ExternalCookie";
    options.ClientId = "placeholder-microsoft-client-id";
    options.ClientSecret = "placeholder-microsoft-client-secret";
    options.Events = customOAuthEvents;
});

builder.Services.AddReverseProxy()
    .LoadFromMemory(GetRoutes(), GetClusters())
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            transformContext.ProxyRequest.Headers.Remove("X-Client-Id");
            transformContext.ProxyRequest.Headers.Add("X-Client-Id", "BFF");

            var accessToken = await transformContext.HttpContext.GetTokenAsync("access-token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                transformContext.ProxyRequest.Headers.Remove("Authorization");
                transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
            }
        });
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("wasm", policy =>
    {
        policy.WithOrigins("https://localhost:7180", "http://localhost:5203")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("wasm");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.Run();

static ClusterConfig[] GetClusters()
{
    return new[]
    {
        new ClusterConfig
        {
            ClusterId = "GatewayCluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "Gateway", new DestinationConfig { Address = "https://localhost:7262" } }
            }
        }
    };
}

static RouteConfig[] GetRoutes()
{
    return new[]
    {
        new RouteConfig
        {
            RouteId = "ToGateway",
            ClusterId = "GatewayCluster",
            Match = new RouteMatch { Path = "/api/{**catch-all}" }
        }
    };
}
