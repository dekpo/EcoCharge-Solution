using EcoCharge.Application.ChargingStations.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargingStations.Commands.CreateChargingStation;

public sealed record CreateChargingStationCommand(string Name, string Location, decimal MaxPowerKw)
    : IRequest<ChargingStationDto>;
