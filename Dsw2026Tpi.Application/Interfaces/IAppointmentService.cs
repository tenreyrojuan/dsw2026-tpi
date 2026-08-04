using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentModel.Response> AddAppointment (AppointmentModel.Request request);
    Task<IEnumerable<AppointmentModel.Response>> GetPatientAppointment(long dni);
    Task DeleteAppointment(Guid id);
}
