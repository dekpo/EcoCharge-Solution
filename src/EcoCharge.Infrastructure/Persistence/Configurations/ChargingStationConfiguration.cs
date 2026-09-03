using EcoCharge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoCharge.Infrastructure.Persistence.Configurations;

public class ChargingStationConfiguration : IEntityTypeConfiguration<ChargingStation>
{
    public void Configure(EntityTypeBuilder<ChargingStation> builder)
    {
        builder.ToTable("ChargingStations");
        builder.HasKey(station => station.Id);

        builder.Property(station => station.Name).IsRequired().HasMaxLength(100);
        builder.Property(station => station.Location).IsRequired().HasMaxLength(150);
        builder.Property(station => station.MaxPowerKw).HasColumnType("decimal(8,2)");
        builder.Property(station => station.Status).HasConversion<string>().HasMaxLength(20);
    }
}
