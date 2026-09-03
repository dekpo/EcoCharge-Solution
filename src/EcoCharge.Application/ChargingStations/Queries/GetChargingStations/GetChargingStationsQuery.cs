using EcoCharge.Application.ChargingStations.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargingStations.Queries.GetChargingStations;

public sealed record GetChargingStationsQuery : IRequest<IReadOnlyList<ChargingStationDto>>;
