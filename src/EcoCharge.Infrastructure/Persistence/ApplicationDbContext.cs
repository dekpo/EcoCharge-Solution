using EcoCharge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoCharge.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ChargingStation> ChargingStations => Set<ChargingStation>();
    public DbSet<ChargeSession> ChargeSessions => Set<ChargeSession>();
    public DbSet<ConsumptionReading> ConsumptionReadings => Set<ConsumptionReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
