using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;

namespace Dsw2026Tpi.CrossCutting.Helpers;

public static class Parsers
{
    public static DayOfWeek ParseDayOfWeek(this string day)
    {
        if (Enum.TryParse<DayOfWeek>(day, true, out var weekDay))
            return weekDay;

        return day.ToLower() switch
        {
            "domingo" => DayOfWeek.Sunday,
            "lunes" => DayOfWeek.Monday,
            "martes" => DayOfWeek.Tuesday,
            "miercoles" or "miércoles" => DayOfWeek.Wednesday,
            "jueves" => DayOfWeek.Thursday,
            "viernes" => DayOfWeek.Friday,
            "sabado" or "sábado" => DayOfWeek.Saturday,
            _ => throw new ValidationException().WithDetail(nameof(day), Issue.INVALID_DAY)
        };
    }
}