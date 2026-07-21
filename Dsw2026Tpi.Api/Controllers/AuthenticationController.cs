using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[Route("auth")]
public class AuthenticationController : AppController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    /// <summary>
    /// Se esta enviando al servicio el DTOs
    /// de la request. Deberiamos cambiar request por
    /// un DTO independiente a la Api?
    /// </summary>
    [HttpPost("admin/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromQuery] RegisterModel.Request request)
    {
        var result = await _authenticationService.Register(request);
        return Ok(result.Email); 
    }

    [HttpPost("admin/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginAdminModel.Request request)
    {
        var result = await _authenticationService.LoginAdmin(request);
        return Ok(result);
    }
}
