using MediatR;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Task.AirAstana.Domain.Enums;

namespace Task.AirAstana.Application.Features.Flights.Commands.CreateFlight;

public class CreateFlightCommand : IRequest<FlightDto>
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public FlightStatus Status { get; set; }
}