using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Enums;
using FluentAssertions;
using Task.AirAstana.Domain.Exceptions;
using Xunit;

namespace Domain.UnitTests.Entities;

public class FlightTests
{
    [Fact]
    public void UpdateStatus_WhenFlightNotCancelled_ShouldUpdateStatus()
    {
        var flight = new Flight
        {
            Status = FlightStatus.InTime
        };

        flight.UpdateStatus(FlightStatus.Delayed);

        flight.Status.Should().Be(FlightStatus.Delayed);
    }

    [Fact]
    public void UpdateStatus_WhenFlightIsCancelled_ShouldThrowInvalidOperationException()
    {
        var flight = new Flight
        {
            Status = FlightStatus.Cancelled
        };

        var act = () => flight.UpdateStatus(FlightStatus.InTime);

        act.Should().Throw<DomainException>() .WithMessage("Не удается обновить статус отмененного рейса");
    }

}