using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IPersistence _persistence;

    public AppointmentService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task AddAppointment(AppointmentModel.Request request)
    {
        var dni = request.Patient.Dni;
        if (dni.Length < 7 || dni.Length > 10)
            throw new ValidationException();

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 5)
            throw new ValidationException();

        var doctor = await _persistence.GetById<Doctor>(request.DoctorId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var patient = await _persistence.First<Patient>(p => p.Dni == dni)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var timeSlot = await _persistence.GetById<TimeSlot>(request.TimeSlotId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        if (timeSlot.Date < hoy)
            throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR));

        var existingAppointment = await _persistence.First<Appointment>(a =>
            a.TimeSlotId == request.TimeSlotId &&
            a.TimeSlot.TimeSlotState == TimeSlotState.AVAILABLE);
        if (existingAppointment != null)
            throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, "El slot de tiempo ya está reservado.");

        var newAppointment = new Appointment(patient, timeSlot, request.Reason);
        _ = await _persistence.Add<Appointment>(newAppointment); 
    }

    public async Task<IEnumerable<AppointmentModel.Response>> GetPatientAppointment(string dni)
    {
        var appointment = await _persistence.GetFiltered<Appointment>(
            a => a.Patient.Dni == dni && a.AppointmentState == AppointmentState.ATTENDED, 
            nameof (Patient), nameof(TimeSlot));
        
        return appointment is null ? [] : 
            appointment.Select(a => new AppointmentModel.Response(a.Patient.Name, a.TimeSlot.Date, a.TimeSlot.StartingTime));

    }

    public async Task DeleteAppointment (Guid id)
    {
        var patient = await _persistence.GetById<Patient>(id)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        _ = await _persistence.Delete<Patient>(patient);
    }
}
