using EcoCharge.Domain.Entities;
using EcoCharge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcoCharge.Infrastructure.Workers;

/// <summary>
/// Native .NET BackgroundService that simulates real-world power draw (kW)
/// for every active charge session, every 5 seconds, and persists a
/// <see cref="ConsumptionReading"/> so the frontend can display a live log.
/// This stands in for real charger telemetry until hardware integration exists.
/// </summary>
public sealed class ChargeSimulationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ChargeSimulationWorker> logger) : BackgroundService
{
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(5);
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ChargeSimulationWorker started (tick interval: {Interval}).", TickInterval);

        using var timer = new PeriodicTimer(TickInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await SimulateTickAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "ChargeSimulationWorker tick failed.");
            }
        }
    }

    private async Task SimulateTickAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var activeSessions = await dbContext.ChargeSessions
            .Where(session => session.StoppedAtUtc == null)
            .ToListAsync(cancellationToken);

        if (activeSessions.Count == 0)
        {
            return;
        }

        var stationsById = await dbContext.ChargingStations
            .Where(station => activeSessions.Select(session => session.ChargingStationId).Contains(station.Id))
            .ToDictionaryAsync(station => station.Id, cancellationToken);

        foreach (var session in activeSessions)
        {
            if (!stationsById.TryGetValue(session.ChargingStationId, out var station))
            {
                continue;
            }

            // Simulate a realistic draw between 30% and 100% of the station's max power,
            // with a little jitter so the live log looks organic rather than static.
            var minPower = station.MaxPowerKw * 0.3m;
            var simulatedPowerKw = minPower + (decimal)_random.NextDouble() * (station.MaxPowerKw - minPower);
            simulatedPowerKw = Math.Round(simulatedPowerKw, 2);

            session.RecordConsumption(simulatedPowerKw, TickInterval);

            dbContext.ConsumptionReadings.Add(new ConsumptionReading(
                session.Id,
                session.ChargingStationId,
                session.CurrentPowerKw,
                session.TotalEnergyKwh));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("Simulated consumption for {Count} active session(s).", activeSessions.Count);
    }
}
