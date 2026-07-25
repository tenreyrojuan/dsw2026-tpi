using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Dtos;


public record DoctorModel
{
    public record Request(string Name, string LicenseNumber, Guid SpecialityId);
    public record Response(Guid Id, string Name, string LicenseNumber, SpecialityDto? Speciality);
    public record SpecialityDto(Guid? SpecialityId, string? Name);
}
public record DoctorApiModel
{
    public record Request(string Name, string LicenseNumber, Guid SpecialityId);
    public record Response(Guid Id, string Name, string LicenseNumber, DoctorModel.SpecialityDto? Speciality);
}
public record DoctorAvailabilityModel
{
    public record Request(Guid Id);
    public record Response(string Day, string StartTime, string EndTime);
}
public record DoctorUpdateModel
{
    public record Request(Guid Id,string Name, string LicenseNumber, Guid SpecialityId);
}