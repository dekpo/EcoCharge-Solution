using EcoCharge.Application.ChargeSessions.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Queries.GetActiveChargeSessions;

public sealed record GetActiveChargeSessionsQuery : IRequest<IReadOnlyList<ChargeSessionDto>>;
