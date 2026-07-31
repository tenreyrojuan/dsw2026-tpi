using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

public class SpecialityService : ISpecialityService
{
    private readonly IPersistence _persistence;

    public SpecialityService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<Pagination<SpecialityModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var specialities = await _persistence.Paginate<Speciality, string>(pageSize, pageIndex,
                                                   e => e.Deleted == false && (string.IsNullOrWhiteSpace(name) ||
                                                   e.Name.Contains(name)), x => x.Name);

        return specialities.Map(e => new SpecialityModel.Response(e.Id, e.Name, e.Description));
    }

    public async Task AddSpeciality(string name, string description)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name),ValidationErrors.INVALID_NAME);

        var speciality= new Speciality(name, description);
        _ = await _persistence.Add<Speciality>(speciality);
    }
    
    public async Task UpdateSpeciality(Guid id, string name, string description)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name),ValidationErrors.INVALID_NAME);

        var speciality = await _persistence.GetById<Speciality>(id)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        speciality.UpdateSpeciality(name, description);

        _ = await _persistence.Update<Speciality>(speciality);
    }
    
    public async Task DeleteSpeciality(Guid id)
    {
        var speciality = await _persistence.GetById<Speciality>(id)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        _ = await _persistence.Delete<Speciality>(speciality);
    }

}
