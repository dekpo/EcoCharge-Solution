using EcoCharge.Domain.Entities;

namespace EcoCharge.Application.Common.Interfaces;

public interface IChargingStationRepository
{
    Task<ChargingStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ChargingStation>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(ChargingStation station, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
