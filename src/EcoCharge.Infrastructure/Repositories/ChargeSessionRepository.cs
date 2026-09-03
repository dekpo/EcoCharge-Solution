using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using EcoCharge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoCharge.Infrastructure.Repositories;

public class ChargeSessionRepository(ApplicationDbContext dbContext) : IChargeSessionRepository
{
    public Task<ChargeSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.ChargeSessions.FirstOrDefaultAsync(session => session.Id == id, cancellationToken);

    public Task<ChargeSession?> GetActiveByStationIdAsync(Guid chargingStationId, CancellationToken cancellationToken) =>
        dbContext.ChargeSessions.FirstOrDefaultAsync(
            session => session.ChargingStationId == chargingStationId && session.StoppedAtUtc == null,
            cancellationToken);

    public async Task<IReadOnlyList<ChargeSession>> GetActiveSessionsAsync(CancellationToken cancellationToken) =>
        await dbContext.ChargeSessions
            .Where(session => session.StoppedAtUtc == null)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ChargeSession session, CancellationToken cancellationToken) =>
        await dbContext.ChargeSessions.AddAsync(session, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
