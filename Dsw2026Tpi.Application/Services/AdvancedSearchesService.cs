using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Specifications;

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
        var spec = new TodaysAppointmentsSpecification(date);
        var todaysAppointments = await _persistence.GetFiltered(spec);

        return todaysAppointments is null ? [] :
            todaysAppointments.Select(
                a => new AppointmentModel.Response(a.Patient.FullName, a.AvailabilitySlot.Date, a.AvailabilitySlot.StartingTime));
    }

    public async Task<Pagination<AdvancedSearchesModel.AppointmentSearchResponse>> SearchAppointments(int pageSize, int pageIndex, Guid specialtyId, Guid doctorId, long dni, DateOnly date)
    {

        var doctorExists = await _persistence.Any<Doctor>(d => d.Id == doctorId);
        if(!doctorExists)
            throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        var specialtyExists = await _persistence.Any<Specialty>(s => s.Id == specialtyId);
        if(!specialtyExists)
            throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(specialtyId), Issue.ID_NOTFOUND);

        var patientDni = dni.ToString();

        var spec = new DailyAppointmentByDateSpecification(doctorId, specialtyId, date, patientDni);
        var appointments = await _persistence.Paginate<Appointment, DateOnly>(pageSize, pageIndex,spec);

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
