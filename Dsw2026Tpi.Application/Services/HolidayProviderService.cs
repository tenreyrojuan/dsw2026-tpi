using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Data.Extensions;

namespace Dsw2026Tpi.Application.Services;

internal sealed class HolidayProviderService : IHolidayProviderService
{
    public Task<IEnumerable<DateOnly>> GetHolidays()
    {
        var holidays = HolidaySeed.LoadHolidays();
        return Task.FromResult<IEnumerable<DateOnly>>(holidays);
    }
}
