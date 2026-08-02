using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services;

public class AdvancedSearchesService : IAdvancedSearchesService
{
    private readonly IPersistence _persistence;

    public AdvancedSearchesService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<IEnumerable<AppointmentModel.Response>> GetAllDailyAppointments(DateOnly date)
    {
        var todaysAppointments
            = await _persistence.GetFiltered<Appointment>(a => a.TimeSlot.Date == date, nameof(Patient), nameof(TimeSlot));

        return todaysAppointments is null ? [] :
            todaysAppointments.Select(
                a => new AppointmentModel.Response(a.Patient.FullName, a.TimeSlot.Date, a.TimeSlot.StartingTime));
    }
}
