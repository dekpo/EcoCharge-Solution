using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Infrastructure.Persistence;
using EcoCharge.Infrastructure.Repositories;
using EcoCharge.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoCharge.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core (SQLite by default, PostgreSQL when
    /// <c>ConnectionStrings:Provider</c> = "Postgres"), repositories, and the
    /// charge-simulation BackgroundService.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ConnectionStrings:Provider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=ecocharge.db";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IChargingStationRepository, ChargingStationRepository>();
        services.AddScoped<IChargeSessionRepository, ChargeSessionRepository>();
        services.AddScoped<IConsumptionReadingRepository, ConsumptionReadingRepository>();

        services.AddHostedService<ChargeSimulationWorker>();

        return services;
    }
}
