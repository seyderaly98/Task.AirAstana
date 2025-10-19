using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Task.AirAstana.Application.Features.Auth.Commands;
using Task.AirAstana.Infrastructure.Identity;

namespace Task.AirAstana.API.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="command">Данные для входа (Username, Password)</param>
    /// <returns>JWT токен и информация о пользователе</returns>
    /// <response code="200">Успешная авторизация</response>
    /// <response code="400">Неверные учетные данные</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(Task.AirAstana.Application.Features.Auth.Models.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
        
}