using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Dtos;

public record DoctorModel
{
    public record Request(string Name, string LicenseNumber, Guid SpecialityId);
    public record Response(Guid Id, string Name, string LicenseNumber, SpecialityDto? Speciality,int Total);
    public record SpecialityDto(Guid? SpecialityId, string? Name);
    public record AvailabilityDto(string Day, string StartTime, string EndTime);
}
