using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Repositories;
using Task.AirAstana.Domain.Services;

namespace Task.AirAstana.Application.Features.Flights.Commands.CreateFlight;

public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, FlightDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFlightCommandHandler> _logger;

    public CreateFlightCommandHandler(
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<CreateFlightCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<FlightDto> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = new Flight
        {
            Origin = request.Origin,
            Destination = request.Destination,
            Departure = request.Departure,
            Arrival = request.Arrival,
            Status = request.Status
        };

        var createdFlight = await _unitOfWork.Flights.AddAsync(flight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveByPatternAsync("flights:*", cancellationToken);

        _logger.LogInformation(
            "Рейс создан. ID: {FlightId}, Origin: {Origin}, Destination: {Destination}, Пользователь: {Username}, Время: {Time}",
            createdFlight.Id,
            createdFlight.Origin,
            createdFlight.Destination,
            _currentUserService.Username ?? "Система",
            DateTimeOffset.UtcNow);
        return _mapper.Map<FlightDto>(createdFlight);
    }
}