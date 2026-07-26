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
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId);
        if (doctor == null)
            throw new Exception("El doctor elegido no existe");

        var availability = await _persistence.GetById<Availability>(request.AvailabilityId);

        if (availability == null)
            throw new Exception("El turno elegido no existe");

        var existingAppointment = await _persistence.First<Appointment>(a => a.PatientId == request.AvailabilityId);
        if (existingAppointment != null)
            throw new Exception("Este turno ya fue reservado por otro paciente");

        var dni = request.Patient.Dni.ToString();
        if (dni.Length < 7 || dni.Length > 10)
            throw new ArgumentException("El DNI debe tener entre 7 y 10 digitos");

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 5)
            throw new ArgumentException("la razon debe tener al menos 5 caracteres.");
    }

    public async Task<IEnumerable<AppointmentModel.Response>> GetPatientAppointment(string dni)
    {
        var appointment = await _persistence.GetFiltered<Appointment>(
            a => a.Patient.Dni == dni && a.IsActive && a.AppointmentState == AppointmentState.Completed, 
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
