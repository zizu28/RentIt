using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentIt.Shared.Abstractions.Exceptions;

namespace RentIt.Shared.Infrastructure.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var contextFeatures = httpContext.Features.Get<IExceptionHandlerFeature>();
        if (contextFeatures != null)
        {
            httpContext.Response.StatusCode = contextFeatures.Error switch
            {
                ArgumentNullException or ArgumentException => (int)StatusCodes.Status400BadRequest,
                KeyNotFoundException => (int)StatusCodes.Status404NotFound,
                UnauthorizedAccessException => (int)StatusCodes.Status401Unauthorized,
                InvalidOperationException => (int)StatusCodes.Status409Conflict,
                NotImplementedException => (int)StatusCodes.Status501NotImplemented,
                TimeoutException => (int)StatusCodes.Status408RequestTimeout,
                NotFoundException => (int)StatusCodes.Status404NotFound,
                BadRequestException => (int)StatusCodes.Status400BadRequest,
                _ => (int)StatusCodes.Status500InternalServerError
            };
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = contextFeatures.Error.Message,
                Detail = contextFeatures.Error.StackTrace,
                Type = contextFeatures.Error.GetType().Name,
            }, cancellationToken);
        }
        return true;
    }
}
