using Microsoft.AspNetCore.Mvc;

namespace Task.AirAstana.API.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{


    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    [HttpPost("Login")]
    public async Task<IActionResult> Login()
    {
        return Ok();
    }
        
}