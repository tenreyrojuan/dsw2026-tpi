using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface ISpecialtyService
{
    Task<Pagination<SpecialtyModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);

    Task<SpecialtyModel.Response>AddSpecialty(string name, string description);

    Task<SpecialtyModel.Response>UpdateSpecialty(Guid id, string name, string description);

    Task DeleteSpecialty(Guid id);
}
