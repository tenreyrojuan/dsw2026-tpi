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
        return !string.IsNullOrWhiteSpace(name) &&
            (name.Length >= 3 && name.Length <= 100);
    }
    public static bool IsMonthValid(this DateOnly month)
    {
        return month.Month >= 1 && month.Month <= 12;
    }
    public static bool IsYearValid(this DateOnly year)
    {
        return year.Year.Equals(DateTime.Now.Year);
    }
}
