using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using System.Linq.Expressions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;


namespace Dsw2026Tpi.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IPersistence _persistence;

    public DoctorService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    /// <summary>
    /// Lo mismo que en el AutenticationService. Estamos validando los datos de la request y 
    /// ademas estamos haciendo las operaciones de los medicos. Deberiamos desacoplar 
    /// el servicio de Doctores de la validacion de los campos provenientes 
    /// de la Api?
    /// </summary>
    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, 
                                                   d => d.IsActive == true && (string.IsNullOrWhiteSpace(name) ||
                                                   d.Name.Contains(name)), x => x.Name, nameof(Doctor.Speciality));
        
        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }

    public async Task<IEnumerable<DoctorAvailabilityModel.Response>> GetDoctorAvailabilities(DoctorAvailabilityModel.Request request)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.Id)
            ?? throw new EntityNotFoundException(nameof(Doctor));

        Expression<Func<Availability, bool>> predicate = a => a.DoctorId == request.Id
                                                         && a.Month == DateTime.Now.Month 
                                                         && a.Year == DateTime.Now.Year;
                                                         
        var availabilities = await _persistence.GetFiltered<Availability>(predicate); 
        
        // si no hay disponibilidades, se devuelve una lista vacia
        return availabilities is null? [] : 
            availabilities
            .OrderBy(a => a.WeekDay)
            .Select(a => new DoctorAvailabilityModel.Response(
                a.WeekDay.ToString().ToUpper(), // nameof(a.WeekDay) devuelve el valor literal WeekDay
                a.StartingHour.ToString("HH:mm"),
                a.EndingHour.ToString("HH:mm")
                )
            );
    }

    public async Task AddDoctor(DoctorModel.Request request)
    {
        if (!request.Name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(request.Name),
                "El nombre es invalido. El campo debe tener entre 3 y 100 caracteres, y no estar vacio.");
        
        var speciality = await _persistence.First<Speciality>(s => s.Id == request.SpecialityId) 
            ?? throw new EntityNotFoundException(nameof(Speciality));

        var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
        _ = await _persistence.Add<Doctor>(doctor);
    }

    public async Task UpdateDoctor(DoctorUpdateModel.Request request)
    {
        if (!request.Name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(request.Name),
                "El nombre es invalido. El campo debe tener entre 3 y 100 caracteres, y no estar vacio.");

        var doctor = await _persistence.GetById<Doctor>(request.Id,nameof(Speciality));        
        var speciality = await _persistence.GetById<Speciality>(request.Id);

        if (speciality is null || doctor is null)
            throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        doctor.UpdateDoctor(request.Name, request.LicenseNumber, request.SpecialityId);

        _ = await _persistence.Update<Doctor>(doctor);
    }

    public async Task DeleteDoctor(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id) 
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        _ = await _persistence.Delete<Doctor>(doctor);
    }
}
