using FluentValidation;

namespace EcoCharge.Application.ChargingStations.Commands.CreateChargingStation;

public sealed class CreateChargingStationCommandValidator : AbstractValidator<CreateChargingStationCommand>
{
    public CreateChargingStationCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Location)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.MaxPowerKw)
            .GreaterThan(0)
            .LessThanOrEqualTo(500)
            .WithMessage("Maximum power output must be between 0 and 500 kW.");
    }
}
