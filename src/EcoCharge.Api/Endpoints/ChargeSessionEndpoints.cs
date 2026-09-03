using EcoCharge.Application.ChargeSessions.Commands.StartChargeSession;
using EcoCharge.Application.ChargeSessions.Commands.StopChargeSession;
using EcoCharge.Application.ChargeSessions.Queries.GetActiveChargeSessions;
using EcoCharge.Application.ChargeSessions.Queries.GetConsumptionLog;
using MediatR;

namespace EcoCharge.Api.Endpoints;

public static class ChargeSessionEndpoints
{
    public static IEndpointRouteBuilder MapChargeSessionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/stations/{stationId:guid}/start", async (
                Guid stationId, StartChargeSessionRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new StartChargeSessionCommand(stationId, request.VehicleIdentifier);
                var session = await sender.Send(command, cancellationToken);
                return TypedResults.Ok(session);
            })
            .WithName("StartChargeSession")
            .WithTags("Charge Sessions");

        app.MapPost("/api/stations/{stationId:guid}/stop", async (
                Guid stationId, ISender sender, CancellationToken cancellationToken) =>
            {
                var session = await sender.Send(new StopChargeSessionCommand(stationId), cancellationToken);
                return TypedResults.Ok(session);
            })
            .WithName("StopChargeSession")
            .WithTags("Charge Sessions");

        var sessionsGroup = app.MapGroup("/api/sessions").WithTags("Charge Sessions");

        sessionsGroup.MapGet("/active", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var sessions = await sender.Send(new GetActiveChargeSessionsQuery(), cancellationToken);
            return TypedResults.Ok(sessions);
        })
        .WithName("GetActiveChargeSessions");

        sessionsGroup.MapGet("/consumption-log", async (int? take, ISender sender, CancellationToken cancellationToken) =>
        {
            var log = await sender.Send(new GetConsumptionLogQuery(take ?? 25), cancellationToken);
            return TypedResults.Ok(log);
        })
        .WithName("GetConsumptionLog");

        return app;
    }
}

public sealed record StartChargeSessionRequest(string VehicleIdentifier);
