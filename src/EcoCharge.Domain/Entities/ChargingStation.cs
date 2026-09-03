using EcoCharge.Domain.Enums;
using EcoCharge.Domain.Exceptions;

namespace EcoCharge.Domain.Entities;

/// <summary>
/// A physical EV charging station belonging to the fleet.
/// Owns the business rule: a charge session can never start while the
/// station is under <see cref="StationStatus.Maintenance"/>.
/// </summary>
public class ChargingStation
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public decimal MaxPowerKw { get; private set; }
    public StationStatus Status { get; private set; }

    // EF Core materialization constructor.
    private ChargingStation()
    {
    }

    public ChargingStation(string name, string location, decimal maxPowerKw)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("A charging station requires a name.");
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new DomainException("A charging station requires a location.");
        }

        if (maxPowerKw <= 0)
        {
            throw new DomainException("Maximum power output must be greater than zero.");
        }

        Id = Guid.NewGuid();
        Name = name;
        Location = location;
        MaxPowerKw = maxPowerKw;
        Status = StationStatus.Available;
    }

    /// <summary>
    /// Starts a new charge session on this station.
    /// Business rule: impossible to start a charge while the station is in
    /// Maintenance, or while it is already Charging (one session at a time).
    /// </summary>
    public ChargeSession StartChargeSession(string vehicleIdentifier)
    {
        if (Status == StationStatus.Maintenance)
        {
            throw new DomainException($"Station '{Name}' is under maintenance and cannot start a charge session.");
        }

        if (Status == StationStatus.Charging)
        {
            throw new DomainException($"Station '{Name}' already has an active charge session.");
        }

        var session = new ChargeSession(Id, vehicleIdentifier);
        Status = StationStatus.Charging;
        return session;
    }

    /// <summary>Called once the active <see cref="ChargeSession"/> has been stopped.</summary>
    public void FinishChargeSession()
    {
        if (Status != StationStatus.Charging)
        {
            throw new DomainException($"Station '{Name}' has no active charge session to finish.");
        }

        Status = StationStatus.Available;
    }

    public void SetMaintenance()
    {
        if (Status == StationStatus.Charging)
        {
            throw new DomainException($"Cannot put station '{Name}' under maintenance while a charge session is active.");
        }

        Status = StationStatus.Maintenance;
    }

    public void ClearMaintenance()
    {
        if (Status != StationStatus.Maintenance)
        {
            throw new DomainException($"Station '{Name}' is not under maintenance.");
        }

        Status = StationStatus.Available;
    }
}
