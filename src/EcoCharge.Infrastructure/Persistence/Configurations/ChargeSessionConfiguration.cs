using EcoCharge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoCharge.Infrastructure.Persistence.Configurations;

public class ChargeSessionConfiguration : IEntityTypeConfiguration<ChargeSession>
{
    public void Configure(EntityTypeBuilder<ChargeSession> builder)
    {
        builder.ToTable("ChargeSessions");
        builder.HasKey(session => session.Id);

        builder.Property(session => session.VehicleIdentifier).IsRequired().HasMaxLength(64);
        builder.Property(session => session.CurrentPowerKw).HasColumnType("decimal(8,3)");
        builder.Property(session => session.TotalEnergyKwh).HasColumnType("decimal(12,4)");

        builder.HasIndex(session => session.ChargingStationId);
    }
}
