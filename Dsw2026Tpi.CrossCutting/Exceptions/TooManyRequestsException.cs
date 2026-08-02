using Dsw2026Tpi.CrossCutting.Resources;

namespace Dsw2026Tpi.CrossCutting.Exceptions;
/// <summary>
/// Excepción que se lanza cuando se exceden las peticiones.
/// </summary>
public class TooManyRequestsException : AppException
{
    public TooManyRequestsException()
        : base(
            ErrorCodes.TOO_MANY_REQUESTS,
            nameof(ErrorCodes.TOO_MANY_REQUESTS))
    {
    }
}
