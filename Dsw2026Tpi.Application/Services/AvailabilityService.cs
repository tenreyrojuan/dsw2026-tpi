using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Data.Extensions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

internal sealed class AvailabilityService : IAvailabilityService
{
    private readonly IPersistence _persistence;
    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<AvailabilityModel.Response> AddAvailability(AvailabilityModel.Request request)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId, nameof(Doctor.AvailabilityRules))
            ?? throw new EntityNotFoundException(ErrorCodes.ENTITY_NOTFOUND)
            .WithDetail(nameof(request.DoctorId), Issue.ID_NOTFOUND);

        foreach (var day in request.Days)
        {
            if (!ValidationsExtensions.IsTimeRangeValid(day.StartTime, day.EndTime))
                throw new BusinessRuleException()
                    .WithDetail(nameof(day), Issue.INVALID_TIME_ERROR);
        }
        
        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;
        
        var holidays = HolidaySeed.LoadHolidays();

        var days = request.Days
            .Select(d => (d.Day, d.StartTime, d.EndTime))
            .ToList();
        
        var finalAvailabilities = doctor.CreateFinalAvailabilities(days, holidays, currentMonth, currentYear, now);

        var rules = await _persistence.AddRange(finalAvailabilities);

        return new AvailabilityModel.Response(doctor.Id,
            rules.Select(a =>
            new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour, a.EndingHour)));
    }

    public async Task<AvailabilityModel.Response> UpdateAvailability(AvailabilityModel.Request request)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId, nameof(Doctor.AvailabilityRules))
            ?? throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(request.DoctorId), Issue.ID_NOTFOUND);

        foreach (var day in request.Days)
        {
            if (!ValidationsExtensions.IsTimeRangeValid(day.StartTime, day.EndTime))
                throw new BusinessRuleException()
                    .WithDetail(nameof(day), Issue.INVALID_TIME_ERROR);
        }
        
        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;
        
        var availabilitiesToDelete = doctor.GetAvailableMonthlyAvailabilityRules(currentMonth, currentYear);
        _ = await _persistence.RemoveRange<AvailabilityRule>(availabilitiesToDelete);

        var holidays = HolidaySeed.LoadHolidays();
        
        var days = request.Days
            .Select(d => (d.Day, d.StartTime, d.EndTime))
            .ToList();
        
        var finalAvailabilities = doctor.CreateFinalAvailabilities(days, holidays, currentMonth, currentYear, now);

        var rules = await _persistence.AddRange(finalAvailabilities);

        return new AvailabilityModel.Response(doctor.Id,
            rules.Select(
                a => new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour,a.EndingHour)) 
            );
    }
}
