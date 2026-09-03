namespace EcoCharge.Domain.Entities;

/// <summary>
/// An immutable point-in-time reading produced by the simulation
/// BackgroundService for one active <see cref="ChargeSession"/>. Persisted
/// purely so the frontend can display a "live log" of consumption events.
/// </summary>
public class ConsumptionReading
{
    public Guid Id { get; private set; }
    public Guid ChargeSessionId { get; private set; }
    public Guid ChargingStationId { get; private set; }
    public decimal PowerKw { get; private set; }
    public decimal TotalEnergyKwh { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }

    private ConsumptionReading()
    {
    }

    public ConsumptionReading(Guid chargeSessionId, Guid chargingStationId, decimal powerKw, decimal totalEnergyKwh)
    {
        Id = Guid.NewGuid();
        ChargeSessionId = chargeSessionId;
        ChargingStationId = chargingStationId;
        PowerKw = powerKw;
        TotalEnergyKwh = totalEnergyKwh;
        RecordedAtUtc = DateTime.UtcNow;
    }
}
