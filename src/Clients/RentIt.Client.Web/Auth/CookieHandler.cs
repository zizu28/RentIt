using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace RentIt.Client.Web.Auth;

/// <summary>
/// A delegating handler that automatically includes credentials (cookies) 
/// in all outgoing HTTP requests from the Blazor WASM client to the BFF.
/// </summary>
public class CookieHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public CookieHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Instructs the browser Fetch API to include credentials (cookies) in the request
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Prevent infinite loop by not triggering re-auth if the request was to the auth endpoint itself
            if (request.RequestUri != null && !request.RequestUri.AbsolutePath.Contains("/bff/auth/user", StringComparison.OrdinalIgnoreCase))
            {
                var authStateProvider = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<BffAuthenticationStateProvider>(_serviceProvider);
                authStateProvider.NotifyUserAuthenticationStateChanged();
                
                var navManager = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>(_serviceProvider);
                
                // Only redirect if we are not already on the login page
                if (!navManager.Uri.Contains("/login", StringComparison.OrdinalIgnoreCase))
                {
                    var returnUrl = Uri.EscapeDataString(new Uri(navManager.Uri).PathAndQuery);
                    navManager.NavigateTo($"/login?returnUrl={returnUrl}");
                }
            }
        }

        return response;
    }
}
