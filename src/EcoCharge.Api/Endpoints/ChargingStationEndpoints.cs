using EcoCharge.Application.ChargingStations.Commands.CreateChargingStation;
using EcoCharge.Application.ChargingStations.Queries.GetChargingStations;
using MediatR;

namespace EcoCharge.Api.Endpoints;

public static class ChargingStationEndpoints
{
    public static IEndpointRouteBuilder MapChargingStationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stations").WithTags("Charging Stations");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var stations = await sender.Send(new GetChargingStationsQuery(), cancellationToken);
            return TypedResults.Ok(stations);
        })
        .WithName("GetChargingStations");

        group.MapPost("/", async (CreateChargingStationRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateChargingStationCommand(request.Name, request.Location, request.MaxPowerKw);
            var station = await sender.Send(command, cancellationToken);
            return TypedResults.Created($"/api/stations/{station.Id}", station);
        })
        .WithName("CreateChargingStation");

        return app;
    }
}

public sealed record CreateChargingStationRequest(string Name, string Location, decimal MaxPowerKw);
