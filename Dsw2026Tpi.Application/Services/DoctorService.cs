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
                                                   d.Name.Contains(name)), x => x.Name, nameof(Doctor.Specialty));
        
        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialtyDto(d.Specialty?.Id, d.Specialty?.Name)));
    }

    public async Task<IEnumerable<DoctorAvailabilityModel.Response>> GetDoctorAvailabilities(Guid doctorId)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

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
            throw new ValidationException()
                .WithDetail(nameof(request.Name),Issue.INVALID_NAME);

        bool takenLicence = await _persistence.Any<Doctor>(d => d.LicenseNumber.Equals(request.LicenseNumber));
        if (takenLicence)
            throw new ConflictException()
                .WithDetail(nameof(request.LicenseNumber),Issue.DUPLICATE_LICENCE);

        var Specialty = await _persistence.GetById<Specialty>(request.SpecialtyId) 
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(request.SpecialtyId), Issue.ID_NOTFOUND);

        var newDoctor = await _persistence.Add<Doctor>(new Doctor(request.Name, request.LicenseNumber, Specialty));
        return new DoctorModel.Response(newDoctor.Id, newDoctor.Name,newDoctor.LicenseNumber,
            new DoctorModel.SpecialtyDto(newDoctor.Specialty?.Id, newDoctor.Specialty?.Name));
    }

    public async Task<DoctorModel.Response> UpdateDoctor(Guid doctorId,DoctorModel.Request request)
    {
        if (!request.Name.IsNameValid())
            throw new ValidationException()
                .WithDetail(nameof(request.Name),Issue.INVALID_NAME);

        bool specialtyExists = await _persistence.Any<Specialty>(s => s.Id == request.SpecialtyId);
            
        if(!specialtyExists)
            throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(request.SpecialtyId), Issue.ID_NOTFOUND);

        var doctor = await _persistence.GetById<Doctor>(doctorId, nameof(Specialty))
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        doctor.UpdateDoctor(request.Name, request.LicenseNumber, request.SpecialtyId);

        var updatedDoctor = await _persistence.Update<Doctor>(doctor);

        return new DoctorModel.Response(
            updatedDoctor.Id, updatedDoctor.Name, updatedDoctor.LicenseNumber,
            new DoctorModel.SpecialtyDto(updatedDoctor.SpecialtyId, updatedDoctor.Specialty.Name));
    }

    public async Task DeleteDoctor(Guid doctorId)
    {
        var doctor = await _persistence.GetById<Doctor>(doctorId) 
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        doctor.SetDelete();

        _ = await _persistence.Update<Doctor>(doctor);
    }
}
