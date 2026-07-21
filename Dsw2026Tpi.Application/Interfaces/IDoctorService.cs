using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IDoctorService
{
    Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);
    Task<IEnumerable<DoctorModel.AvailabilityDto>> GetDoctorAvailabilities(Guid id,DateOnly date);
    Task AddDoctor(string name, string licenseNumber, Guid specialityId);
    Task UpdateDoctor(Guid id, string name, string licenseNumber, Guid specialityId);
    Task DeleteDoctor(Guid id);

}
