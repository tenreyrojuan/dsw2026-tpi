using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public sealed class DailyAppointmentByDateSpecification : Specification<Appointment>
{
    private static readonly string[] _includes =
    {
        $"{nameof(AvailabilitySlot)}.{nameof(AvailabilityRule)}.{nameof(Doctor)}.{nameof(Specialty)}",
        $"{nameof(Patient)}"
    };
    public DailyAppointmentByDateSpecification(Guid doctorId,Guid specialtyId,DateOnly date,string patientDni) 
        : base(a => a.AvailabilitySlot.AvailabilityRule.DoctorId == doctorId &&
                    a.AvailabilitySlot.AvailabilityRule.Doctor.SpecialtyId == specialtyId &&
                    a.AvailabilitySlot.Date == date &&
                    a.Patient.Dni == patientDni, _includes)
    {
        AddOrderBy(a => a.AvailabilitySlot.Date);
    }
}
