using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAdvancedSearchesService
{
    Task<IEnumerable<AppointmentModel.Response>> GetAllDailyAppointments(DateOnly date);
}
