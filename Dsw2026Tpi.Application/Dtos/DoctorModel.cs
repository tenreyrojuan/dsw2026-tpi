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
    // Aqui deberia devolver id y un array con varios dias, no un array donde siempre esta el id (como dice el endpoint
    public record Response(Guid Id,string Day, string StartTime, string EndTime);
}
