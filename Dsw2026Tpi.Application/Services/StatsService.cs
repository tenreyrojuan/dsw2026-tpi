using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

internal sealed class StatsService : IStatsService
{
    private readonly IPersistence _persistence;

    public StatsService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<StatsModel.Response> GetStats()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var activeDoctors = await _persistence.Count<Doctor>(d => !d.Deleted);

        var activeSpecialties = await _persistence.Count<Specialty>(s => true);

        var bookedAppointmentsToday = await _persistence.Count<Appointment>(
            a => a.AppointmentState == AppointmentState.BOOKED &&
                 a.AvailabilitySlot.Date == today);

        return new StatsModel.Response(activeDoctors, activeSpecialties, bookedAppointmentsToday);
    }
}