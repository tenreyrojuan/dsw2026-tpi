using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public class PatientBookedAppointmentSpecification : Specification<Appointment>
{
    public PatientBookedAppointmentSpecification(string dni, DateOnly today)
        : base(
            a => a.Patient.Dni == dni && a.AppointmentState == AppointmentState.BOOKED &&
                 a.AvailabilitySlot.Date >= today, nameof(Patient), nameof(AvailabilitySlot))
    {
        SetAsNoTracking();
    }
}