using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IHolidayProviderService
{
    Task<IEnumerable<DateOnly>> GetHolidays();
}
