using EcoCharge.Application.ChargingStations.Dtos;
using EcoCharge.Application.Common.Interfaces;
using MediatR;

namespace EcoCharge.Application.ChargingStations.Queries.GetChargingStations;

public sealed class GetChargingStationsQueryHandler(IChargingStationRepository repository)
    : IRequestHandler<GetChargingStationsQuery, IReadOnlyList<ChargingStationDto>>
{
    public async Task<IReadOnlyList<ChargingStationDto>> Handle(GetChargingStationsQuery request, CancellationToken cancellationToken)
    {
        var stations = await repository.GetAllAsync(cancellationToken);
        return stations.Select(ChargingStationDto.FromEntity).ToList();
    }
}
