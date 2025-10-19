using Task.AirAstana.Application.Features.Flights.Commands.UpdateFlightStatus;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Enums;
using Task.AirAstana.Domain.Exceptions;
using Task.AirAstana.Domain.Repositories;
using Task.AirAstana.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.UnitTests.Flights.Commands;

public class UpdateFlightStatusCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<UpdateFlightStatusCommandHandler>> _mockLogger;
    private readonly UpdateFlightStatusCommandHandler _handler;

    public UpdateFlightStatusCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<ICacheService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<UpdateFlightStatusCommandHandler>>();

        _handler = new UpdateFlightStatusCommandHandler(
            _mockUnitOfWork.Object,
            _mockCacheService.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFlightExists_ShouldUpdateStatus()
    {
        var command = new UpdateFlightStatusCommand
        {
            FlightId = 1,
            Status = FlightStatus.Delayed
        };

        var flight = new Flight
        {
            Id = 1,
            Status = FlightStatus.InTime
        };

        _mockUnitOfWork.Setup(x => x.Flights.GetByIdAsync(command.FlightId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        await _handler.Handle(command, CancellationToken.None);

        flight.Status.Should().Be(FlightStatus.Delayed);
        _mockUnitOfWork.Verify(x => x.Flights.UpdateAsync(flight, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync("flights:all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFlightDoesNotExist_ShouldThrowNotFoundException()
    {
        var command = new UpdateFlightStatusCommand
        {
            FlightId = 999,
            Status = FlightStatus.Delayed
        };

        _mockUnitOfWork.Setup(x => x.Flights.GetByIdAsync(command.FlightId, It.IsAny<CancellationToken>())) .ReturnsAsync((Flight?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*Flight*999*");
    }
}