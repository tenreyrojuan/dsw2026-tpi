using Dsw2026Tpi.CrossCutting.Resources;

namespace Dsw2026Tpi.CrossCutting.Exceptions;

/// <summary>
/// Excepción que se lanza cuando se viola una regla de negocio.
/// </summary>
public class BusinessRuleException : AppException
{
    public BusinessRuleException()
        : base(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR))
    {

    }
    public BusinessRuleException(string message, string errorCode)
        : base(message, errorCode)
    {
    }
}
