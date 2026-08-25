using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.Domain.Specifications;

namespace Dsw2026Tpi.Application.Services;

internal sealed class SpecialtyService : ISpecialtyService
{
    private readonly IPersistence _persistence;

    public SpecialtyService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<Pagination<SpecialtyModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var spec = new SpecialtiesByNameSpecification(name);
        var specialties = await _persistence.Paginate<Specialty, string>(pageSize, pageIndex, spec);

        return specialties.Map(e => new SpecialtyModel.Response(e.Id, e.Name, e.Description));
    }

    public async Task<SpecialtyModel.Response> AddSpecialty(string name, string description)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name),Issue.INVALID_NAME);

        if (!description.IsDescriptionValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(description), Issue.INVALID_DESCRIPTION);

        var specialtyExist = await _persistence.Any<Specialty>(s => s.Name.Equals(name));
        if (specialtyExist)
            throw new ConflictException()
                .WithDetail(nameof(name), Issue.DUPLICATE_SPECIALTY);
        
        var specialty = new Specialty(name, description);
        var created = _persistence.Add(specialty);
        _ = await _persistence.SaveChangesAsync();
        
        return new SpecialtyModel.Response(created.Id, created.Name, created.Description);
    }
    
    public async Task<SpecialtyModel.Response>UpdateSpecialty(Guid id, string name, string description)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name),Issue.INVALID_NAME);

        var specialty = await _persistence.GetById<Specialty>(id)
            ?? throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(id), Issue.ID_NOTFOUND);

        specialty.UpdateSpecialty(name, description);

        //var updated = _persistence.Update<Specialty>(specialty);
        _ = await _persistence.SaveChangesAsync();
        
        return new SpecialtyModel.Response(specialty.Id, specialty.Name, specialty.Description);
    }
    
    public async Task DeleteSpecialty(Guid id)
    {
        var specialty = await _persistence.GetById<Specialty>(id)
            ?? throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(id), Issue.ID_NOTFOUND);

        specialty.SetDelete();
        //_ =  _persistence.Update<Specialty>(specialty);
        _ = await _persistence.SaveChangesAsync();
    }

}
