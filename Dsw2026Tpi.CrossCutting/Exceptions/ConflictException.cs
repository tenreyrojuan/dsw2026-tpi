using Dsw2026Tpi.CrossCutting.Resources;

namespace Dsw2026Tpi.CrossCutting.Exceptions;

/// <summary>
/// Excepción que se lanza cuando ocurre un conflicto, por ejemplo al intentar crear un recurso duplicado.
/// </summary>
public class ConflictException : AppException
{
    public ConflictException()
        : base(ErrorCodes.CONFLICT_DUPLICATE_RESOURCE, nameof(ErrorCodes.CONFLICT_DUPLICATE_RESOURCE))
    {
    }
    public ConflictException(string errorCode, string message)
        : base(errorCode, message)
    {
    }
}
