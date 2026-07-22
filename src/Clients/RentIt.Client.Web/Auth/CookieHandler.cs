using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace RentIt.Client.Web.Auth;

/// <summary>
/// A delegating handler that automatically includes credentials (cookies) 
/// in all outgoing HTTP requests from the Blazor WASM client to the BFF.
/// </summary>
public class CookieHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Instructs the browser Fetch API to include credentials (cookies) in the request
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return await base.SendAsync(request, cancellationToken);
    }
}
