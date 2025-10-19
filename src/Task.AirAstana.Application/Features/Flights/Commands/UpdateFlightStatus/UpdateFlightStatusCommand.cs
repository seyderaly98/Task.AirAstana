using MediatR;
using Task.AirAstana.Domain.Enums;

namespace Task.AirAstana.Application.Features.Flights.Commands.UpdateFlightStatus;

public class UpdateFlightStatusCommand : IRequest<Unit>
{
    public int FlightId { get; set; }
    public FlightStatus Status { get; set; }
}