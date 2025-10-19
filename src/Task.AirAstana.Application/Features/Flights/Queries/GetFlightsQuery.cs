using MediatR;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;

namespace Task.AirAstana.Application.Features.Flights.Queries;

public class GetFlightsQuery : IRequest<List<FlightDto>>
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
}