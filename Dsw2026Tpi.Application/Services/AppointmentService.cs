using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IPersistence _persistence;

    public AppointmentService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<AppointmentModel.Response> AddAppointment(AppointmentModel.Request request)
    {
        if (!request.Patient.Dni.IsDniValid())
            throw new ValidationException()
                .WithDetail(nameof(request.Patient.Dni), Issue.INVALID_DNI);

        if (!request.Reason.IsReasonValid())
            throw new ValidationException()
                .WithDetail(nameof(request.Patient.Dni), Issue.INVALID_REASON);

        var doctor = await _persistence.GetById<Doctor>(request.DoctorId)
            ?? throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(request.DoctorId), Issue.ID_NOTFOUND);

        var patient = await _persistence.First<Patient>(p => p.Dni == request.Patient.Dni)
            ?? throw new EntityNotFoundException(nameof(Patient))
            .WithDetail(nameof(request.Patient.Dni), Issue.DNI_NOTFOUND);

        var timeSlot = await _persistence.GetById<TimeSlot>(request.AvailabilitySlotId)
            ?? throw new EntityNotFoundException(nameof(TimeSlot))
            .WithDetail(nameof(request.AvailabilitySlotId), Issue.ID_NOTFOUND);

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (timeSlot.Date < today)
            throw new BusinessRuleException()
                .WithDetail(nameof(timeSlot.Date), Issue.NOTACTUAL_SLOT);

        var existingAppointment = await _persistence.First<Appointment>(a =>
            a.TimeSlotId == request.AvailabilitySlotId &&
            a.TimeSlot.TimeSlotState == TimeSlotState.AVAILABLE);

        if (existingAppointment != null)
            throw new BusinessRuleException()
                .WithDetail(nameof(existingAppointment), Issue.NOTAVAILABLE_SLOT);

        var newAppointment = new Appointment(patient, timeSlot, request.Reason);
        var appointment = await _persistence.Add<Appointment>(newAppointment);

        return new AppointmentModel.Response(appointment.Patient.FullName, today, timeSlot.StartingTime);
    }

    public async Task<IEnumerable<AppointmentModel.Response>> GetPatientAppointment(string dni)
    {
        var appointment = await _persistence.GetFiltered<Appointment>(
            a => a.Patient.Dni == dni && a.AppointmentState == AppointmentState.BOOKED, 
            nameof (Patient), nameof(TimeSlot));
        
        return appointment is null ? [] : 
            appointment.Select(a => new AppointmentModel.Response(
                a.Patient.FullName, a.TimeSlot.Date, a.TimeSlot.StartingTime));
    }

    public async Task<AppointmentModel.Response> DeleteAppointment (Guid availabilityTimeSlotid)
    {
        var appointment = await _persistence.GetById<Appointment>(
            availabilityTimeSlotid, nameof(Patient), nameof(TimeSlot))
                ?? throw new EntityNotFoundException(nameof(Appointment))
                .WithDetail(nameof(availabilityTimeSlotid), Issue.ID_NOTFOUND);


        if (appointment.AppointmentState != AppointmentState.BOOKED)
            throw new BusinessRuleException()
                .WithDetail(nameof(appointment.AppointmentState), Issue.NOT_BOOKEDSLOT);

        appointment = await _persistence.Delete<Appointment>(appointment);

        return new AppointmentModel.Response(
            appointment.Patient.FullName, appointment.TimeSlot.Date,
            appointment.TimeSlot.StartingTime);
    }

}
