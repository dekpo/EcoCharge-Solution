using EcoCharge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoCharge.Infrastructure.Persistence;

/// <summary>
/// Inserts a handful of Swiss demo stations when the database has none,
/// so the dashboard is usable without a manual POST. Idempotent: existing
/// rows are never overwritten or duplicated.
/// </summary>
public static class DemoStationSeeder
{
    public static async Task SeedIfEmptyAsync(
        ApplicationDbContext dbContext,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.ChargingStations.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.ChargingStations.AddRange(
            new ChargingStation("Lausanne Flon", "Place de l'Europe, Lausanne", 22m),
            new ChargingStation("Bern Wankdorf", "Stade de Suisse, Bern", 50m),
            new ChargingStation("Zurich Hardbrücke", "Hardbrücke, Zurich", 150m));

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} demo charging stations.", 3);
    }
}
