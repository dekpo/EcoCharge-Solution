using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using EcoCharge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoCharge.Infrastructure.Repositories;

public class ChargingStationRepository(ApplicationDbContext dbContext) : IChargingStationRepository
{
    public Task<ChargingStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.ChargingStations.FirstOrDefaultAsync(station => station.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ChargingStation>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.ChargingStations.AsNoTracking().OrderBy(station => station.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(ChargingStation station, CancellationToken cancellationToken) =>
        await dbContext.ChargingStations.AddAsync(station, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
