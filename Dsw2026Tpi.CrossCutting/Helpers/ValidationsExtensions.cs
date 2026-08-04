using System.Text.RegularExpressions;

namespace Dsw2026Tpi.CrossCutting.Helpers;

public static class ValidationsExtensions
{
    public const string EmailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$";
    public static bool IsEmailValid(this string? email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email, EmailPattern);
    }
    public static bool IsNameValid(this string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length >= 3 && name.Length <= 100;
    }
    public static bool IsDniValid(this string? dni)
    {
        return !string.IsNullOrWhiteSpace(dni) && dni.Length >= 7 && dni.Length <= 8;
    }
    /// <summary>
    /// Al presentarse una incongruencia entre la longitud de los dnis del paciente
    /// en el login y al anotar un turno, se crea el metodo temporal de abajo
    /// </summary>
    public static bool IsDniValidAppointment(this string? dni)
    {
        return !string.IsNullOrWhiteSpace(dni) && dni.Length >= 7 && dni.Length <= 10;
    }
    public static bool IsReasonValid(this string? reason)
    {
        return !string.IsNullOrWhiteSpace(reason) && reason.Length >= 5;
    }
    public static bool IsDescriptionValid(this string? description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Length >= 10 && description.Length <= 100;
    }
}
