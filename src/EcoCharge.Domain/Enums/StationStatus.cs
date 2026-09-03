namespace EcoCharge.Domain.Enums;

/// <summary>
/// Lifecycle status of a <see cref="Entities.ChargingStation"/>.
/// </summary>
public enum StationStatus
{
    /// <summary>Idle and ready to accept a new charge session.</summary>
    Available = 0,

    /// <summary>Currently powering an active <see cref="Entities.ChargeSession"/>.</summary>
    Charging = 1,

    /// <summary>Taken out of service; cannot start new charge sessions.</summary>
    Maintenance = 2,
}
