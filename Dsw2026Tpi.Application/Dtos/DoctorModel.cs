using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Dtos;


public record DoctorModel
{
    public record Request(string Name, string LicenseNumber, Guid SpecialtyId);
    public record Response(Guid Id, string Name, string LicenseNumber, SpecialtyDto? Specialty);
    public record SpecialtyDto(Guid? Id, string? Name);
}
public record DoctorAvailabilityModel
{
    public record Response(string Day, string StartTime, string EndTime);
}
