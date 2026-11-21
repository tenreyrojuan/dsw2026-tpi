using Dsw2026Tpi.CrossCutting.Resources;

namespace Dsw2026Tpi.CrossCutting.Exceptions;

public sealed class PreconditionException : AppException
{
    public PreconditionException(string errorCode, string message)
        : base(errorCode, message)
    {
    }
}