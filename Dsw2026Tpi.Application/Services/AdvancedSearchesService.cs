using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

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
            = await _persistence.GetFiltered<Appointment>(a => a.AvailabilitySlot.Date == date, nameof(Patient), nameof(AvailabilitySlot));

        return todaysAppointments is null ? [] :
            todaysAppointments.Select(
                a => new AppointmentModel.Response(a.Patient.FullName, a.AvailabilitySlot.Date, a.AvailabilitySlot.StartingTime));
    }
}
