using EcoCharge.Application.ChargeSessions.Dtos;
using EcoCharge.Application.Common.Interfaces;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Queries.GetConsumptionLog;

public sealed class GetConsumptionLogQueryHandler(
    IConsumptionReadingRepository readingRepository,
    IChargingStationRepository stationRepository)
    : IRequestHandler<GetConsumptionLogQuery, IReadOnlyList<ConsumptionLogEntryDto>>
{
    public async Task<IReadOnlyList<ConsumptionLogEntryDto>> Handle(GetConsumptionLogQuery request, CancellationToken cancellationToken)
    {
        var readings = await readingRepository.GetRecentAsync(request.Take, cancellationToken);
        var stations = await stationRepository.GetAllAsync(cancellationToken);
        var stationNamesById = stations.ToDictionary(station => station.Id, station => station.Name);

        return readings
            .Select(reading => new ConsumptionLogEntryDto(
                reading.Id,
                reading.ChargingStationId,
                stationNamesById.GetValueOrDefault(reading.ChargingStationId, "Unknown station"),
                reading.PowerKw,
                reading.TotalEnergyKwh,
                reading.RecordedAtUtc))
            .ToList();
    }
}
