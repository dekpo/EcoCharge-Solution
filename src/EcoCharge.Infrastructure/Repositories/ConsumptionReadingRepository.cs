using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using EcoCharge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoCharge.Infrastructure.Repositories;

public class ConsumptionReadingRepository(ApplicationDbContext dbContext) : IConsumptionReadingRepository
{
    public async Task AddAsync(ConsumptionReading reading, CancellationToken cancellationToken) =>
        await dbContext.ConsumptionReadings.AddAsync(reading, cancellationToken);

    public async Task<IReadOnlyList<ConsumptionReading>> GetRecentAsync(int take, CancellationToken cancellationToken) =>
        await dbContext.ConsumptionReadings
            .AsNoTracking()
            .OrderByDescending(reading => reading.RecordedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
