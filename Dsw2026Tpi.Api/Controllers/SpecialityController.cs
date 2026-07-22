using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("specialities")]
[Authorize]
public class SpecialityController : AppController
{
    private readonly ISpecialityService _service;

    public SpecialityController(ISpecialityService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageSize, [FromQuery] int pageIndex, [FromQuery] string? name = null)
    {
        var result = await _service.GetAll(pageSize, pageIndex, name);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Policies.AdminPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSpeciality([FromBody] SpecialityModel.Request request)
    {
        await _service.AddSpeciality(request.Name, request.Description);
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSpeciality([FromRoute] Guid id, [FromBody] SpecialityModel.Request request)
    {
        await _service.UpdateSpeciality(id, request.Name, request.Description);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSpeciality([FromRoute] Guid id)
    {
        await _service.DeleteSpeciality(id);
        return Ok();
    }
}
