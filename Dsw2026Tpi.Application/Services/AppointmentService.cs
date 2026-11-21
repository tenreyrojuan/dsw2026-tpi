using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

internal sealed class AppointmentService : IAppointmentService
{
    private readonly IPersistence _persistence;

    public AppointmentService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<AppointmentModel.Response> AddAppointment(AppointmentModel.Request request)
    {
        // se escribe appointment y se añade otro metodo el helpers por una incongruencia en
        // la longitud del dni del paciente en el login y al añadir el turno
        // Nota: este metodo de extension se eliminara a futuro y quedará "IsDniValid" en su lugar
        if (!request.Patient.Dni.IsDniValidAppointment())
            throw new ValidationException()
                .WithDetail(nameof(request.Patient.Dni), Issue.INVALID_DNI_APPOINTMENT);

        if (!request.Reason.IsReasonValid())
            throw new ValidationException()
                .WithDetail(nameof(request.Reason), Issue.INVALID_REASON);

        var doctor = await _persistence.GetById<Doctor>(request.DoctorId)
            ?? throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(request.DoctorId), Issue.ID_NOTFOUND);

        var patient = await _persistence.First<Patient>(p => p.Dni == request.Patient.Dni)
            ?? throw new EntityNotFoundException(nameof(Patient))
            .WithDetail(nameof(request.Patient.Dni), Issue.DNI_NOTFOUND);

        var availabilitySlot = await _persistence.GetById<AvailabilitySlot>(request.AvailabilitySlotId)
            ?? throw new EntityNotFoundException(nameof(AvailabilitySlot))
            .WithDetail(nameof(request.AvailabilitySlotId), Issue.ID_NOTFOUND);

        availabilitySlot.CheckIfValid();

        var newAppointment = Appointment.Create(patient, availabilitySlot, request.Reason);

        var appointment = await _persistence.Add<Appointment>(newAppointment);

        availabilitySlot.Book(newAppointment);
        _ = await _persistence.Update<AvailabilitySlot>(availabilitySlot);

        return new AppointmentModel.Response(appointment.Patient.FullName, availabilitySlot.Date, availabilitySlot.StartingTime);
    }

    public async Task<IEnumerable<AppointmentModel.PatientAppointmentResponse>> GetPatientAppointment(long patientDni)
    {
        var dni = patientDni.ToString();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var appointments = await _persistence.GetFiltered<Appointment>(
            a => a.Patient.Dni == dni &&
                 a.AppointmentState == AppointmentState.BOOKED &&
                 a.AvailabilitySlot.Date >= today,
            nameof(Patient),
            $"{nameof(AvailabilitySlot)}.{nameof(AvailabilityRule)}.{nameof(Doctor)}.{nameof(Specialty)}");

        return appointments is null ? [] :
            appointments
                .OrderBy(a => a.AvailabilitySlot.Date)
                .ThenBy(a => a.AvailabilitySlot.StartingTime)
                .Select(a => new AppointmentModel.PatientAppointmentResponse(
                    a.Id,
                    a.AvailabilitySlot.AvailabilityRule.Doctor.Name,
                    a.AvailabilitySlot.AvailabilityRule.Doctor.Specialty.Name,
                    a.AvailabilitySlot.Date,
                    a.AvailabilitySlot.StartingTime));
    }

    public async Task DeleteAppointment (Guid appointmentId)
    {
        var appointment 
            = await _persistence.GetById<Appointment>(appointmentId, nameof(Patient), nameof(AvailabilitySlot))
                ?? throw new EntityNotFoundException(nameof(Appointment))
                    .WithDetail(nameof(appointmentId), Issue.ID_NOTFOUND);

        if (!appointment.IsBooked)
            throw new BusinessRuleException()
                .WithDetail(nameof(appointment.AppointmentState), Issue.UNBOOKED_SLOT);

        appointment.Cancel();
        _ = await _persistence.Update<Appointment>(appointment);
    }

}
