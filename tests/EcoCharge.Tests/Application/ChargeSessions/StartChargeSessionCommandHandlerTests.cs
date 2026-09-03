using EcoCharge.Application.ChargeSessions.Commands.StartChargeSession;
using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using EcoCharge.Domain.Exceptions;
using Moq;

namespace EcoCharge.Tests.Application.ChargeSessions;

public class StartChargeSessionCommandHandlerTests
{
    private readonly Mock<IChargingStationRepository> _stationRepositoryMock = new();
    private readonly Mock<IChargeSessionRepository> _sessionRepositoryMock = new();

    private StartChargeSessionCommandHandler CreateHandler() =>
        new(_stationRepositoryMock.Object, _sessionRepositoryMock.Object);

    [Fact]
    public async Task Handle_WhenStationIsAvailable_StartsSessionAndReturnsDto()
    {
        // Arrange
        var station = new ChargingStation("Bern Depot - Bay 2", "Bern, BE", 50m);
        _stationRepositoryMock
            .Setup(repo => repo.GetByIdAsync(station.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(station);

        var handler = CreateHandler();
        var command = new StartChargeSessionCommand(station.Id, "ZH-123456");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(station.Id, result.ChargingStationId);
        Assert.Equal(command.VehicleIdentifier, result.VehicleIdentifier);
        Assert.True(result.IsActive);
        Assert.Equal(Domain.Enums.StationStatus.Charging, station.Status);

        _sessionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ChargeSession>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenStationIsUnderMaintenance_ThrowsDomainExceptionAndNeverPersistsSession()
    {
        // Arrange: business rule under test — cannot start a charge while
        // the station is in Maintenance. The rule lives on ChargingStation
        // itself (see EcoCharge.Domain), this test proves the Application
        // handler correctly propagates it instead of swallowing it.
        var station = new ChargingStation("Zurich Airport - Bay 5", "Zurich, ZH", 100m);
        station.SetMaintenance();

        _stationRepositoryMock
            .Setup(repo => repo.GetByIdAsync(station.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(station);

        var handler = CreateHandler();
        var command = new StartChargeSessionCommand(station.Id, "ZH-999999");

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        _sessionRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<ChargeSession>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
