using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAdvancedSearchesService
{
    Task<IEnumerable<AppointmentModel.Response>> GetAllDailyAppointments(DateOnly date);
    Task<Pagination<AdvancedSearchesModel.AppointmentSearchResponse>> SearchAppointments(int pageSize, int pageIndex, Guid specialtyId , Guid doctorId, long dni, DateOnly date);
}
