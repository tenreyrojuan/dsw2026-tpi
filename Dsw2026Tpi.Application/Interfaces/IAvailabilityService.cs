using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAvailabilityService
{
    Task<AvailabilityModel.Response> AddAvailability(AvailabilityModel.Request request);
    Task<AvailabilityModel.Response> UpdateAvailability(AvailabilityModel.Request request);
}
