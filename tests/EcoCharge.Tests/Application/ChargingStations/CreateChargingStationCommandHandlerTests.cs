using EcoCharge.Application.ChargingStations.Commands.CreateChargingStation;
using EcoCharge.Application.Common.Interfaces;
using EcoCharge.Domain.Entities;
using Moq;

namespace EcoCharge.Tests.Application.ChargingStations;

public class CreateChargingStationCommandHandlerTests
{
    private readonly Mock<IChargingStationRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_WithValidCommand_PersistsStationAndReturnsMatchingDto()
    {
        // Arrange
        var handler = new CreateChargingStationCommandHandler(_repositoryMock.Object);
        var command = new CreateChargingStationCommand("Lausanne HQ - Bay 1", "Lausanne, VD", 22m);

        ChargingStation? capturedStation = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ChargingStation>(), It.IsAny<CancellationToken>()))
            .Callback<ChargingStation, CancellationToken>((station, _) => capturedStation = station)
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedStation);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Location, result.Location);
        Assert.Equal(command.MaxPowerKw, result.MaxPowerKw);
        Assert.Equal(Domain.Enums.StationStatus.Available, result.Status);

        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ChargingStation>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
