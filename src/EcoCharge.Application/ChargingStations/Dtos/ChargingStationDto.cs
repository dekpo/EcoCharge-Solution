using EcoCharge.Domain.Entities;
using EcoCharge.Domain.Enums;

namespace EcoCharge.Application.ChargingStations.Dtos;

public sealed record ChargingStationDto(
    Guid Id,
    string Name,
    string Location,
    decimal MaxPowerKw,
    StationStatus Status)
{
    public static ChargingStationDto FromEntity(ChargingStation station) =>
        new(station.Id, station.Name, station.Location, station.MaxPowerKw, station.Status);
}
