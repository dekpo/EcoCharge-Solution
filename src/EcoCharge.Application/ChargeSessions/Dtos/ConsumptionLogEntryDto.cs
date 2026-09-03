namespace EcoCharge.Application.ChargeSessions.Dtos;

public sealed record ConsumptionLogEntryDto(
    Guid Id,
    Guid ChargingStationId,
    string StationName,
    decimal CurrentPowerKw,
    decimal TotalEnergyKwh,
    DateTime RecordedAtUtc);
