using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dsw2026Tpi.Application.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/appointment")]
[Authorize(Policies.AdminPolicy)]
[EnableRateLimiting(Policies.AdminPolicy)]
public class AdvancedSearchesController : AppController
{
    private readonly IAdvancedSearchesService _service;
    public AdvancedSearchesController(IAdvancedSearchesService service)
    {
        _service = service;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetAllDailyAppointments([FromQuery] DateOnly date)
    {
        var result = await _service.GetAllDailyAppointments(date);
        return Ok(result);
    }
}
