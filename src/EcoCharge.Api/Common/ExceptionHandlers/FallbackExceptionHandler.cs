using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoCharge.Api.Common.ExceptionHandlers;

/// <summary>
/// Last-resort handler: any exception not already handled becomes a generic
/// 500 ProblemDetails response. Never leaks exception details to the client.
/// </summary>
public sealed class FallbackExceptionHandler(ILogger<FallbackExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}.", httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "Please try again later or contact support if the problem persists.",
            Type = "https://ecocharge.example/problems/internal-server-error",
        }, cancellationToken);

        return true;
    }
}
