using EcoCharge.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoCharge.Api.Common.ExceptionHandlers;

/// <summary>
/// Translates business-rule violations (e.g. "cannot start a charge while
/// in maintenance") into a standardized RFC 7807 ProblemDetails 409 response.
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Business rule violation",
            Detail = domainException.Message,
            Type = "https://ecocharge.example/problems/domain-rule-violation",
        }, cancellationToken);

        return true;
    }
}
