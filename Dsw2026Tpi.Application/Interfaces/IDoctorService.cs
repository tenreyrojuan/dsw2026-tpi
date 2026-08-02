using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IDoctorService
{
    Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);
    Task<IEnumerable<DoctorAvailabilityModel.Response>> GetDoctorAvailabilities(Guid doctord);
    Task<DoctorModel.Response> AddDoctor(DoctorModel.Request request);
    Task<DoctorModel.Response> UpdateDoctor(Guid doctorId,DoctorModel.Request request);
    Task DeleteDoctor(Guid doctorId);

}
