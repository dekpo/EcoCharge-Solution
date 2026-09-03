using FluentValidation;

namespace EcoCharge.Application.ChargeSessions.Commands.StartChargeSession;

public sealed class StartChargeSessionCommandValidator : AbstractValidator<StartChargeSessionCommand>
{
    public StartChargeSessionCommandValidator()
    {
        RuleFor(command => command.ChargingStationId).NotEmpty();

        RuleFor(command => command.VehicleIdentifier)
            .NotEmpty()
            .MaximumLength(64);
    }
}
