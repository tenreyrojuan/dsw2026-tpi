using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AppointmentModel
{
    public record Request(Guid DoctorId, Guid AvailabilitySlotId, PatientDto Patient, string Reason);
    public record Response(string PatientName, DateOnly Date, TimeOnly StartingTime);
    public record PatientDto(string Dni);
    
}
