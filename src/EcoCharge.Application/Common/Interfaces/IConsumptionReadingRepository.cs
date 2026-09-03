using EcoCharge.Domain.Entities;

namespace EcoCharge.Application.Common.Interfaces;

public interface IConsumptionReadingRepository
{
    Task AddAsync(ConsumptionReading reading, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConsumptionReading>> GetRecentAsync(int take, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
