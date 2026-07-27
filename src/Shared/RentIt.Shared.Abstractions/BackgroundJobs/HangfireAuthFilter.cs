using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace RentIt.Shared.Abstractions.BackgroundJobs;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // Allow local requests for development
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        if (remoteIp == null || System.Net.IPAddress.IsLoopback(remoteIp) || 
            (httpContext.Connection.LocalIpAddress != null && remoteIp.Equals(httpContext.Connection.LocalIpAddress)))
        {
            return true;
        }

        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
