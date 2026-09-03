namespace EcoCharge.Domain.Exceptions;

/// <summary>
/// Raised when a business invariant of the Domain layer is violated
/// (e.g. starting a charge on a station under maintenance).
/// Mapped to HTTP 409 Conflict by the Api's global exception handler.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
