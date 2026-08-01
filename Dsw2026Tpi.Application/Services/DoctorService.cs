using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using System.Linq.Expressions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using System.Globalization;


namespace Dsw2026Tpi.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IPersistence _persistence;

    public DoctorService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, 
                                                   d => d.IsActive == true && (string.IsNullOrWhiteSpace(name) ||
                                                   d.Name.Contains(name)), x => x.Name, nameof(Doctor.Speciality));
        
        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }

    public async Task<IEnumerable<DoctorAvailabilityModel.Response>> GetDoctorAvailabilities(Guid doctorId)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(nameof(Doctor));

        Expression<Func<Availability, bool>> predicate = a => a.DoctorId == doctorId
                                                         && a.Month == DateTime.Now.Month 
                                                         && a.Year == DateTime.Now.Year;
                                                         
        var availabilities = await _persistence.GetFiltered<Availability>(predicate); 
        
        // si no hay disponibilidades, se devuelve una lista vacia
        return availabilities is null? [] : 
            availabilities
            .OrderBy(a => a.WeekDay)
            .Select(a => new DoctorAvailabilityModel.Response(
                CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(a.WeekDay).ToUpper(),
                a.StartingHour.ToString("HH:mm"),
                a.EndingHour.ToString("HH:mm")
                )
            );
    }

    public async Task<DoctorModel.Response> AddDoctor(DoctorModel.Request request)
    {
        if (!request.Name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(request.Name),ValidationErrors.INVALID_NAME);
        
        var speciality = await _persistence.First<Speciality>(s => s.Id == request.SpecialityId) 
            ?? throw new EntityNotFoundException(nameof(Speciality));

        var newDoctor = await _persistence.Add<Doctor>(new Doctor(request.Name, request.LicenseNumber, speciality));
        return new DoctorModel.Response(newDoctor.Id, newDoctor.Name,newDoctor.LicenseNumber,
            new DoctorModel.SpecialityDto(newDoctor.Speciality?.Id, newDoctor.Speciality?.Name));
    }

    public async Task UpdateDoctor(Guid doctorId,DoctorModel.Request request)
    {
        if (!request.Name.IsNameValid())
            throw new ValidationException(ErrorCodes.VALIDATION_ERROR, nameof(ErrorCodes.VALIDATION_ERROR))
                .WithDetail(nameof(request.Name),ValidationErrors.INVALID_NAME);

        var doctor = await _persistence.GetById<Doctor>(doctorId, nameof(Speciality));        
        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);

        if (speciality is null || doctor is null)
            throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        doctor.UpdateDoctor(request.Name, request.LicenseNumber, request.SpecialityId);

        _ = await _persistence.Update<Doctor>(doctor);
    }

    public async Task DeleteDoctor(Guid doctorId)
    {
        var doctor = await _persistence.GetById<Doctor>(doctorId) 
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND);

        _ = await _persistence.Delete<Doctor>(doctor);
    }
}
