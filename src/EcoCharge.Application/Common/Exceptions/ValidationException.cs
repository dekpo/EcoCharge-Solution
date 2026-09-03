using FluentValidation.Results;

namespace EcoCharge.Application.Common.Exceptions;

/// <summary>
/// Aggregates FluentValidation failures for a single request. Mapped to
/// HTTP 400 with a ProblemDetails payload by the Api's global exception handler.
/// </summary>
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName, failure => failure.ErrorMessage)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }
}
