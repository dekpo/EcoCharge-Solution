using EcoCharge.Application.ChargeSessions.Dtos;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Queries.GetConsumptionLog;

public sealed record GetConsumptionLogQuery(int Take = 25) : IRequest<IReadOnlyList<ConsumptionLogEntryDto>>;
