using EcoCharge.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoCharge.Api.Common.ExceptionHandlers;

/// <summary>
/// Translates FluentValidation failures (raised via the MediatR
/// ValidationBehavior) into a standardized RFC 7807 ValidationProblemDetails
/// 400 response, mirroring ASP.NET Core's built-in model-validation shape.
/// </summary>
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(validationException.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://ecocharge.example/problems/validation-error",
        }, cancellationToken);

        return true;
    }
}
