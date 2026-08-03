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

    public async Task<Pagination<AdvancedSearchesModel.AppointmentSearchResponse>> SearchAppointments(int pageSize, int pageIndex, Guid specialtyId, Guid doctorId, int dni, DateOnly date)
    {
        var doctor = await _persistence.GetById<Doctor>(doctorId, nameof(Specialty), nameof(AvailabilitySlot));

        var appointments = await _persistence.Paginate<Appointment, DateOnly>(pageSize, pageIndex, e => 
        );

        return appointments.Map(e => new AdvancedSearchesModel.AppointmentSearchResponse(
            e.Id,
            e.AppointmentState.ToString(),
            new AdvancedSearchesModel.PatientDto(
                e.Patient.Dni,
                $"{e.Patient.FullName}"
            ),
            new AdvancedSearchesModel.DoctorDto(
                e.AvailabilitySlot.AvailabilityRule.Doctor.Id,
                $"{e.AvailabilitySlot.AvailabilityRule.Doctor.Name}",
                new AdvancedSearchesModel.SpecialtyDto(
                    e.AvailabilitySlot.AvailabilityRule.Doctor.Specialty.Id,
                    e.AvailabilitySlot.AvailabilityRule.Doctor.Specialty.Name
                )
            )
        ));
    }
}
