using EcoCharge.Application.ChargeSessions.Dtos;
using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Exceptions;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Commands.StopChargeSession;

public sealed class StopChargeSessionCommandHandler(
    IChargingStationRepository stationRepository,
    IChargeSessionRepository sessionRepository)
    : IRequestHandler<StopChargeSessionCommand, ChargeSessionDto>
{
    public async Task<ChargeSessionDto> Handle(StopChargeSessionCommand request, CancellationToken cancellationToken)
    {
        var station = await stationRepository.GetByIdAsync(request.ChargingStationId, cancellationToken)
            ?? throw new DomainException($"Charging station '{request.ChargingStationId}' was not found.");

        var session = await sessionRepository.GetActiveByStationIdAsync(request.ChargingStationId, cancellationToken)
            ?? throw new DomainException($"Station '{station.Name}' has no active charge session.");

        session.Stop();
        station.FinishChargeSession();

        await sessionRepository.SaveChangesAsync(cancellationToken);
        await stationRepository.SaveChangesAsync(cancellationToken);

        return ChargeSessionDto.FromEntity(session);
    }
}
