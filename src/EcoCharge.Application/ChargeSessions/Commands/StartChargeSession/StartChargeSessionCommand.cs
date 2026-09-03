using EcoCharge.Application.ChargeSessions.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Commands.StartChargeSession;

public sealed record StartChargeSessionCommand(Guid ChargingStationId, string VehicleIdentifier)
    : IRequest<ChargeSessionDto>;
