using Microsoft.AspNetCore.Authorization;
using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task.AirAstana.Application.Features.Flights.Commands.CreateFlight;
using Task.AirAstana.Application.Features.Flights.Commands.UpdateFlightStatus;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Task.AirAstana.Application.Features.Flights.Queries;

namespace Task.AirAstana.API.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список всех рейсов 
    /// </summary>
    /// <param name="origin">Пункт отправления (опционально)</param>
    /// <param name="destination">Пункт назначения (опционально)</param>
    /// <returns>Список рейсов, отсортированных по времени прилёта</returns>
    /// <response code="200">Список рейсов</response>
    /// <response code="401">Не авторизован</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFlights([FromQuery] string? origin, [FromQuery] string? destination)
    {
        var query = new GetFlightsQuery
        {
            Origin = origin,
            Destination = destination
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Создать новый рейс (только для Moderator)
    /// </summary>
    /// <param name="command">Данные нового рейса</param>
    /// <returns>Созданный рейс</returns>
    /// <response code="201">Рейс успешно создан</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Moderator)</response>
    [HttpPost]
    [Authorize(Policy = "Moderator")]
    [ProducesResponseType(typeof(FlightDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFlight([FromBody] CreateFlightCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFlights), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить статус рейса (только для Moderator)
    /// </summary>
    /// <param name="id">ID рейса</param>
    /// <param name="command">Новый статус рейса</param>
    /// <returns>Без содержимого</returns>
    /// <response code="204">Статус успешно обновлён</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="404">Рейс не найден</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Moderator)</response>
    [HttpPut("{id}/status")]
    [Authorize(Policy = "Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateFlightStatus(int id, [FromBody] UpdateFlightStatusCommand command)
    {
        if (id != command.FlightId)
            return BadRequest("ID в URL не совпадает с ID в теле запроса");
        await _mediator.Send(command);
        return NoContent();
    }
    
}