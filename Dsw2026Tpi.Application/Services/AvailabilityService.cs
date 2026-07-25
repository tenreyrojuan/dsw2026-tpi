using Azure.Core;
using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IPersistence _persistence;
    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
    }
    
    public async Task AddAvailability(AvailabilityModel.Request request)
    {
        await CUAvailability(request, isUpdate: false);
    }

    public async Task UpdateAvailability(AvailabilityModel.Request request)
    {
        await CUAvailability(request, isUpdate: true);
    }
    private async Task CUAvailability(AvailabilityModel.Request request, bool isUpdate)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId)
    ?? throw new EntityNotFoundException(nameof(Doctor));

        if (isUpdate)
        {
            var existingAvailabilities = await _persistence.GetFiltered<Availability>(
                a => a.DoctorId == request.DoctorId && a.Month == DateTime.Now.Month && a.Year == DateTime.Now.Year);

            if (existingAvailabilities != null)
            {
                foreach (var existing in existingAvailabilities)
                {
                    await _persistence.Delete(existing);
                }
            }
        }
        var days = request.Days;
        ICollection<Availability> updatedAvailabilities = [];
        foreach (var day in days)
        {
            if (day.StartTime > day.EndTime)
                throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR));

            var availabilities = doctor.Availabilities;

            foreach (var availability in availabilities)
            {
                if (availability.HasOverlappingSchedules(day.StartTime, day.EndTime))
                    throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, nameof(ErrorCodes.BUSINESS_ERROR));

                var listTimeSlots = availability.CreateTimeSlots(availability, day.StartTime, day.EndTime);
                updatedAvailabilities.Add(availability);
            }

        }

        doctor.UpdateDoctorAvailabilities(updatedAvailabilities);
        _ = await _persistence.Update<Doctor>(doctor);
    }

}
