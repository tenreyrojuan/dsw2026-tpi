using System;
using System.Collections.Generic;
using System.Text;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Dtos;

public record class AvailabilityModel
{
    public record Request(Guid DoctorId, IEnumerable<DayScheduleRequest> Days);
    public record Response(Guid DoctorId, IEnumerable<DayScheduleRequest> Days);
    public record DayScheduleRequest(string Day, TimeOnly StartTime, TimeOnly EndTime);
}
