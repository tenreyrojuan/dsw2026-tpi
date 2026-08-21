using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System.Linq.Expressions;

namespace Dsw2026Tpi.Application.Services;

internal sealed class AdvancedSearchesService : IAdvancedSearchesService
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

    public async Task<Pagination<AdvancedSearchesModel.AppointmentSearchResponse>> SearchAppointments(int pageSize, int pageIndex, Guid specialtyId, Guid doctorId, long dni, DateOnly date)
    {

        var doctor = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        var specialty = await _persistence.GetById<Specialty>(specialtyId)
            ?? throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(specialtyId), Issue.ID_NOTFOUND);

        var patientDni = dni.ToString();
        Expression<Func<Appointment, bool>> predicate
            = a => a.AvailabilitySlot.AvailabilityRule.DoctorId == doctorId &&
                   a.AvailabilitySlot.AvailabilityRule.Doctor.SpecialtyId == specialtyId &&
                   a.AvailabilitySlot.Date == date &&
                   a.Patient.Dni == patientDni;

        Expression<Func<Appointment, DateOnly>> sortOrder
            = a => a.AvailabilitySlot.Date;

        string[] includes = {
            $"{nameof(AvailabilitySlot)}.{nameof(AvailabilityRule)}.{nameof(Doctor)}.{nameof(Specialty)}",
            $"{nameof(Patient)}"
        };
        var appointments = await _persistence.Paginate<Appointment, DateOnly>(pageSize, pageIndex, predicate, sortOrder, includes);

        return appointments.Map(a => new AdvancedSearchesModel.AppointmentSearchResponse(
            a.Id,
            a.AppointmentState.ToString(),
            new AdvancedSearchesModel.PatientDto(dni, a.Patient.FullName),
                new AdvancedSearchesModel.DoctorDto(
                    a.AvailabilitySlot.AvailabilityRule.DoctorId,
                    a.AvailabilitySlot.AvailabilityRule.Doctor.Name,
                    new AdvancedSearchesModel.SpecialtyDto(
                        a.AvailabilitySlot.AvailabilityRule.Doctor.SpecialtyId,
                        a.AvailabilitySlot.AvailabilityRule.Doctor.Specialty.Name
                    ))));
    }
}
