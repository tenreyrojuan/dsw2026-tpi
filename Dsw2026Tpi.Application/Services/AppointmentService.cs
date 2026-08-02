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
        if (request.Patient.Dni.IsDniValid())
            throw new ValidationException();

        if (request.Reason.IsReasonValid())
            throw new ValidationException();

        var doctor = await _persistence.GetById<Doctor>(request.DoctorId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var patient = await _persistence.First<Patient>(p => p.Dni == request.Patient.Dni)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var timeSlot = await _persistence.GetById<TimeSlot>(request.AvailabilityTimeSlotId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (timeSlot.Date < today)
            throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR));

        var existingAppointment = await _persistence.First<Appointment>(a =>
            a.TimeSlotId == request.AvailabilityTimeSlotId &&
            a.TimeSlot.TimeSlotState == TimeSlotState.AVAILABLE);

        if (existingAppointment != null)
            throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR));

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
            availabilityTimeSlotid,nameof(Patient),nameof(TimeSlot))
                ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        appointment = await _persistence.Delete<Appointment>(appointment);

        return new AppointmentModel.Response(
            appointment.Patient.FullName, appointment.TimeSlot.Date,
            appointment.TimeSlot.StartingTime);
    }
}
