using EcoCharge.Application.ChargeSessions.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Commands.StopChargeSession;

public sealed record StopChargeSessionCommand(Guid ChargingStationId) : IRequest<ChargeSessionDto>;
