using EcoCharge.Application.ChargeSessions.Dtos;
using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Exceptions;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Commands.StartChargeSession;

/// <summary>
/// Orchestrates starting a charge session. The actual "no charging while in
/// maintenance" invariant lives on <see cref="Domain.Entities.ChargingStation"/>
/// itself — this handler only fetches the aggregate, delegates to it, and persists.
/// </summary>
public sealed class StartChargeSessionCommandHandler(
    IChargingStationRepository stationRepository,
    IChargeSessionRepository sessionRepository)
    : IRequestHandler<StartChargeSessionCommand, ChargeSessionDto>
{
    public async Task<ChargeSessionDto> Handle(StartChargeSessionCommand request, CancellationToken cancellationToken)
    {
        var station = await stationRepository.GetByIdAsync(request.ChargingStationId, cancellationToken)
            ?? throw new DomainException($"Charging station '{request.ChargingStationId}' was not found.");

        var session = station.StartChargeSession(request.VehicleIdentifier);

        await sessionRepository.AddAsync(session, cancellationToken);
        await sessionRepository.SaveChangesAsync(cancellationToken);
        await stationRepository.SaveChangesAsync(cancellationToken);

        return ChargeSessionDto.FromEntity(session);
    }
}
