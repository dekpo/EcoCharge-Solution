using EcoCharge.Domain.Entities;

namespace EcoCharge.Application.ChargeSessions.Dtos;

public sealed record ChargeSessionDto(
    Guid Id,
    Guid ChargingStationId,
    string VehicleIdentifier,
    DateTime StartedAtUtc,
    DateTime? StoppedAtUtc,
    decimal CurrentPowerKw,
    decimal TotalEnergyKwh,
    bool IsActive)
{
    public static ChargeSessionDto FromEntity(ChargeSession session) => new(
        session.Id,
        session.ChargingStationId,
        session.VehicleIdentifier,
        session.StartedAtUtc,
        session.StoppedAtUtc,
        session.CurrentPowerKw,
        session.TotalEnergyKwh,
        session.IsActive);
}
