using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

/// <summary>
/// Clase base para configuraciones generales de controladores
/// </summary>
[ApiController]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
[AllowAnonymous]
public abstract class AppController : ControllerBase
{
}

