using System.Text.Json;

namespace Dsw2026Tpi.Data.Extensions;

public static class HolidaySeed
{
    public static IEnumerable<DateOnly> LoadHolidays()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Sources", "holidays.json");
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<DateOnly>>(json) ?? [];
    }
}
