using EcoCharge.Domain.Exceptions;

namespace EcoCharge.Domain.Entities;

/// <summary>
/// A single charging session of one vehicle at one <see cref="ChargingStation"/>.
/// Instances are only ever created through <see cref="ChargingStation.StartChargeSession"/>,
/// which enforces the "no charging while in maintenance" invariant.
/// </summary>
public class ChargeSession
{
    public Guid Id { get; private set; }
    public Guid ChargingStationId { get; private set; }
    public string VehicleIdentifier { get; private set; } = string.Empty;
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? StoppedAtUtc { get; private set; }
    public decimal CurrentPowerKw { get; private set; }
    public decimal TotalEnergyKwh { get; private set; }

    public bool IsActive => StoppedAtUtc is null;

    // EF Core materialization constructor.
    private ChargeSession()
    {
    }

    internal ChargeSession(Guid chargingStationId, string vehicleIdentifier)
    {
        if (string.IsNullOrWhiteSpace(vehicleIdentifier))
        {
            throw new DomainException("A charge session requires a vehicle identifier.");
        }

        Id = Guid.NewGuid();
        ChargingStationId = chargingStationId;
        VehicleIdentifier = vehicleIdentifier;
        StartedAtUtc = DateTime.UtcNow;
        CurrentPowerKw = 0m;
        TotalEnergyKwh = 0m;
    }

    /// <summary>
    /// Called by the Infrastructure BackgroundService every simulation tick to
    /// record instantaneous power draw and accumulate consumed energy.
    /// </summary>
    public void RecordConsumption(decimal powerKw, TimeSpan elapsedSinceLastTick)
    {
        if (!IsActive)
        {
            throw new DomainException("Cannot record consumption on a charge session that has already stopped.");
        }

        if (powerKw < 0)
        {
            throw new DomainException("Power draw cannot be negative.");
        }

        CurrentPowerKw = powerKw;
        TotalEnergyKwh += powerKw * (decimal)elapsedSinceLastTick.TotalHours;
    }

    /// <summary>Ends the session. The owning station transitions back to Available.</summary>
    public void Stop()
    {
        if (!IsActive)
        {
            throw new DomainException("Charge session is already stopped.");
        }

        StoppedAtUtc = DateTime.UtcNow;
        CurrentPowerKw = 0m;
    }
}
