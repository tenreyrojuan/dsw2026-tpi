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
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name),doctors.Total));
    }

    public async Task<IEnumerable<DoctorModel.AvailabilityDto>> GetDoctorAvailabilities(Guid id,DateOnly date)
    {
        if (!date.IsMonthValid() || !date.IsYearValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR));
        
        Doctor? doctor = await _persistence.GetById<Doctor>(id)
            ?? throw new EntityNotFoundException(nameof(Doctor));

        Expression<Func<Availability, bool>> predicate = a => a.DoctorId == id 
                                                         && a.Month == date.Month 
                                                         && a.Year == date.Year;
        // si no hay disponibilidades, se devuelve vacio
        var availabilities = await _persistence.GetFiltered<Availability>(predicate) ?? []; 

        
        return availabilities
            .OrderBy(a => a.WeekDay)
            .Select(a => new DoctorModel.AvailabilityDto(
                nameof(a.WeekDay).ToUpper(), // Añadir comprobacion en los endpoints de availability
                a.StartingHour.ToString("HH:mm"),
                a.EndingHour.ToString("HH:mm")
                )
            );
    }

    public async Task AddDoctor(string name, string licenseNumber, Guid specialityId)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name),
                "El nombre es invalido. El campo debe tener entre 3 y 100 caracteres, y no estar vacio.");
        
        var speciality = await _persistence.First<Speciality>(s => s.Id == specialityId) 
            ?? throw new EntityNotFoundException(nameof(Speciality));

        var doctor = new Doctor(name, licenseNumber, speciality);
        _ = await _persistence.Add<Doctor>(doctor);
    }

    public async Task UpdateDoctor(Guid id, string name, string licenseNumber, Guid specialityId)
    {
        if (!name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(name), "El nombre es invalido"); ;

        var speciality = await _persistence.GetById<Speciality>(specialityId);
        var doctor = await _persistence.GetById<Doctor>(id);

        if (speciality is null || doctor is null)
            throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        doctor.UpdateDoctor(name, licenseNumber, speciality);

        _ = await _persistence.Update<Doctor>(doctor);
    }
    /// <summary>
    /// Podriamos evaluar hacer un metodo que verifique si el medico (o cualquier entidad del dominio)
    /// existe en lugar de traer todos los campos para hacer aqui la validacion. Capaz que haya ya 
    /// algun metodo de la persistencia que permita esto...
    /// </summary>
    public async Task DeleteDoctor(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id) 
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        _ = await _persistence.Delete<Doctor>(doctor);
    }

}
