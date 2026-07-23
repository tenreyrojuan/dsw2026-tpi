using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface ISpecialityService
{
    Task<Pagination<SpecialityModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);

    Task AddSpeciality(string name, string description);

    Task UpdateSpeciality(Guid id, string name, string description);

    Task DeleteSpeciality(Guid id);
}
