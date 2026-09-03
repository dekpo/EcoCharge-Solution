using EcoCharge.Domain.Entities;

namespace EcoCharge.Application.Common.Interfaces;

public interface IChargeSessionRepository
{
    Task<ChargeSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ChargeSession?> GetActiveByStationIdAsync(Guid chargingStationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ChargeSession>> GetActiveSessionsAsync(CancellationToken cancellationToken);

    Task AddAsync(ChargeSession session, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
