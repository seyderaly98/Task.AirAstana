using MediatR;
using Microsoft.Extensions.Logging;
using Task.AirAstana.Domain.Exceptions;
using Task.AirAstana.Domain.Repositories;
using Task.AirAstana.Domain.Services;

namespace Task.AirAstana.Application.Features.Flights.Commands.UpdateFlightStatus;

public class UpdateFlightStatusCommandHandler : IRequestHandler<UpdateFlightStatusCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateFlightStatusCommandHandler> _logger;

    public UpdateFlightStatusCommandHandler( IUnitOfWork unitOfWork, ICacheService cacheService, ICurrentUserService currentUserService, ILogger<UpdateFlightStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
    {
        var flight = await _unitOfWork.Flights.GetByIdAsync(request.FlightId, cancellationToken);

        if (flight == null)
            throw new NotFoundException(nameof(Domain.Entities.Flight), request.FlightId);

        var oldStatus = flight.Status;
        flight.UpdateStatus(request.Status);

        await _unitOfWork.Flights.UpdateAsync(flight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync("flights:all", cancellationToken);

        _logger.LogInformation(
            "Статус рейса изменён. ID: {FlightId}, Старый статус: {OldStatus}, Новый статус: {NewStatus}, Пользователь: {Username}, Время: {Time}",
            flight.Id,
            oldStatus,
            flight.Status,
            _currentUserService.Username ?? "Система",
            DateTimeOffset.UtcNow);

        return Unit.Value;
    }
}