using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("api/slots")]
[Authorize(Policy = Policies.AdminPolicy)]
[AllowAnonymous]
public class SlotController : AppController
{
    private readonly ISlotService _service;

    public SlotController(ISlotService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery]int pageSize,[FromQuery] int pageIndex)
    {
        var result = await _service.GetTodaysSlots(pageSize, pageIndex);
        return Ok(result);
    }
}