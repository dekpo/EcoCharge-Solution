using EcoCharge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoCharge.Infrastructure.Persistence.Configurations;

public class ConsumptionReadingConfiguration : IEntityTypeConfiguration<ConsumptionReading>
{
    public void Configure(EntityTypeBuilder<ConsumptionReading> builder)
    {
        builder.ToTable("ConsumptionReadings");
        builder.HasKey(reading => reading.Id);

        builder.Property(reading => reading.PowerKw).HasColumnType("decimal(8,3)");
        builder.Property(reading => reading.TotalEnergyKwh).HasColumnType("decimal(12,4)");

        builder.HasIndex(reading => reading.RecordedAtUtc);
    }
}
