using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Task.AirAstana.API.Presentation.Controllers;

[ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlightsController : ControllerBase
    {

        /// <summary>
        /// Получить список всех рейсов с возможностью фильтрации
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            return null;
        }

        /// <summary>
        /// Создать новый рейс (только для Moderator)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateFlight()
        {
            return null;
        }

        /// <summary>
        /// Обновить статус рейса (только для Moderator)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateFlightStatus(int id)
        {
            return null;
        }
    }