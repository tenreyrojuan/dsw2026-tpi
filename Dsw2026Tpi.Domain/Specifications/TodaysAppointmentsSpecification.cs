using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public sealed class TodaysAppointmentsSpecification : Specification<Appointment>
{
    public TodaysAppointmentsSpecification(DateOnly date) 
        : base(a => a.AvailabilitySlot.Date == date, nameof(Patient), nameof(AvailabilitySlot))
    {
    }
}