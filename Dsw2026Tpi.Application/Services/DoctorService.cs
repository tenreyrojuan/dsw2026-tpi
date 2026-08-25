using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using System.Linq.Expressions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using System.Globalization;
using Dsw2026Tpi.Domain.Specifications;


namespace Dsw2026Tpi.Application.Services;

internal sealed class DoctorService : IDoctorService
{
    private readonly IPersistence _persistence;

    public DoctorService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var spec = new DoctorsByNameSpecification(name, nameof(Doctor.Specialty));
            var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, spec);
        
        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialtyDto(d.Specialty?.Id, d.Specialty?.Name)));
    }

    public async Task<IEnumerable<DoctorAvailabilityModel.Response>> GetDoctorAvailabilities(Guid doctorId)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        /*Expression<Func<AvailabilityRule, bool>> predicate = a => a.DoctorId == doctorId
                                                         && a.Month == DateTime.Now.Month 
                                                         && a.Year == DateTime.Now.Year;
        */
        var spec = new DoctorMonthlyAvailabilityRulesSpecification(doctorId);
        var availabilities = await _persistence.GetFiltered(spec); 
        
        // si no hay disponibilidades, se devuelve una lista vacia
        return availabilities is null? [] : 
            availabilities
            .OrderBy(a => a.WeekDay)
            .Select(a => new DoctorAvailabilityModel.Response(
                doctor.Id,
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

        var specialty = await _persistence.GetById<Specialty>(request.SpecialtyId) 
            ?? throw new EntityNotFoundException(nameof(Specialty))
            .WithDetail(nameof(request.SpecialtyId), Issue.ID_NOTFOUND);

        var newDoctor = _persistence.Add(new Doctor(request.Name, request.LicenseNumber, specialty));
        _ = await _persistence.SaveChangesAsync();
            
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
        
        bool takenLicence = await _persistence.Any<Doctor>(d => d.LicenseNumber.Equals(request.LicenseNumber));
        if (takenLicence)
            throw new ConflictException()
                .WithDetail(nameof(request.LicenseNumber),Issue.DUPLICATE_LICENCE);

        var doctor = await _persistence.GetById<Doctor>(doctorId,nameof(Specialty))
                     ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
                         .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        doctor.UpdateDoctor(request.Name, request.LicenseNumber, request.SpecialtyId);

        //var updatedDoctor = _persistence.Update<Doctor>(doctor);
        _ = await _persistence.SaveChangesAsync();
        
        return new DoctorModel.Response(
            doctor.Id, doctor.Name, doctor.LicenseNumber,
            new DoctorModel.SpecialtyDto(doctor.SpecialtyId, doctor.Specialty.Name));
    }

    public async Task DeleteDoctor(Guid doctorId)
    {
        var doctor = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(doctorId), Issue.ID_NOTFOUND);

        doctor.SetDelete();

        //_ = await _persistence.Update<Doctor>(doctor);
        _ = await _persistence.SaveChangesAsync();
    }
}
