using Task.AirAstana.Application.Features.Flights.Commands.CreateFlight;
using Task.AirAstana.Application.Features.Flights.Models;
using AutoMapper;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Enums;
using Task.AirAstana.Domain.Repositories;
using Task.AirAstana.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Xunit;

namespace Application.UnitTests.Flights.Commands;

public class CreateFlightCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CreateFlightCommandHandler>> _mockLogger;
    private readonly CreateFlightCommandHandler _handler;

    public CreateFlightCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<ICacheService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CreateFlightCommandHandler>>();

        _handler = new CreateFlightCommandHandler(
            _mockUnitOfWork.Object,
            _mockCacheService.Object,
            _mockCurrentUserService.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenCommandIsValid_ShouldCreateFlight()
    {
        var command = new CreateFlightCommand
        {
            Origin = "Алматы",
            Destination = "Астана",
            Departure = DateTimeOffset.UtcNow.AddDays(1),
            Arrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            Status = FlightStatus.InTime
        };

        var createdFlight = new Flight
        {
            Id = 1,
            Origin = command.Origin,
            Destination = command.Destination,
            Departure = command.Departure,
            Arrival = command.Arrival,
            Status = command.Status
        };

        var flightDto = new FlightDto
        {
            Id = 1,
            Origin = command.Origin,
            Destination = command.Destination,
            Status = "InTime"
        };

        _mockUnitOfWork.Setup(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockMapper.Setup(x => x.Map<FlightDto>(It.IsAny<Flight>()))
            .Returns(flightDto);

        _mockCurrentUserService.Setup(x => x.Username)
            .Returns("admin");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Origin.Should().Be(command.Origin);
        result.Destination.Should().Be(command.Destination);

        _mockUnitOfWork.Verify(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync("flights:all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFlightCreated_ShouldInvalidateCache()
    {
        var command = new CreateFlightCommand
        {
            Origin = "Алматы",
            Destination = "Астана",
            Departure = DateTimeOffset.UtcNow.AddDays(1),
            Arrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            Status = FlightStatus.InTime
        };

        var createdFlight = new Flight { Id = 1 };
        var flightDto = new FlightDto { Id = 1 };

        _mockUnitOfWork.Setup(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdFlight);
        _mockMapper.Setup(x => x.Map<FlightDto>(It.IsAny<Flight>())).Returns(flightDto);

        await _handler.Handle(command, CancellationToken.None);
        _mockCacheService.Verify( x => x.RemoveAsync("flights:all", It.IsAny<CancellationToken>()), Times.Once);
    }
}
