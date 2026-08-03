namespace Dsw2026Tpi.Application.Dtos;

public record AdvancedSearchesModel
{
    public record AppointmentSearchResponse(Guid AppointmentsId, string AppointmentsStatus, PatientDto Patient, DoctorDto Doctor);
    public record PatientDto(string Dni, string FullName);
    public record DoctorDto(Guid DoctorId, string Name, SpecialtyDto Specialty);
    public record SpecialtyDto(Guid SpecialtyId, string Name);
}