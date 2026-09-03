using EcoCharge.Application.ChargingStations.Dtos;
using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using MediatR;

namespace EcoCharge.Application.ChargingStations.Commands.CreateChargingStation;

public sealed class CreateChargingStationCommandHandler(IChargingStationRepository repository)
    : IRequestHandler<CreateChargingStationCommand, ChargingStationDto>
{
    public async Task<ChargingStationDto> Handle(CreateChargingStationCommand request, CancellationToken cancellationToken)
    {
        var station = new ChargingStation(request.Name, request.Location, request.MaxPowerKw);

        await repository.AddAsync(station, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ChargingStationDto.FromEntity(station);
    }
}
