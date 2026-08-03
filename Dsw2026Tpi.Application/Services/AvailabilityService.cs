using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System.Numerics;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService : IAvailabilityService
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

        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        ICollection<AvailabilityRule> finalAvailabilities 
            = CreateFinalAvailabilities(request.Days, doctor, currentMonth, currentYear,now);
        
        var disps = await _persistence.AddRange(finalAvailabilities);

        return new AvailabilityModel.Response(doctor.Id,
            disps.Select(a => 
            new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour, a.EndingHour)));
    }

    public async Task<AvailabilityModel.Response> UpdateAvailability(AvailabilityModel.Request request)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId, nameof(Doctor.AvailabilityRules))
            ?? throw new EntityNotFoundException(nameof(Doctor))
            .WithDetail(nameof(request.DoctorId), Issue.ID_NOTFOUND);

        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        // se listan las disponibilidades que son de este mes, este año y ademas no tienen slots ocupados
        var availabilitiesToDelete = doctor.AvailabilityRules
           .Where(a => a.Month == currentMonth && a.Year == currentYear && !a.AvailabilitySlots.Any(s => s.AvailabilitySlotState == AvailabilitySlotState.BOOKED))
           .ToArray();
        _ = await _persistence.RemoveRange<AvailabilityRule>(availabilitiesToDelete);

        ICollection<AvailabilityRule> finalAvailabilities = 
            CreateFinalAvailabilities(request.Days,doctor,currentMonth,currentYear,now);

        var disps = await _persistence.AddRange(finalAvailabilities);

        return new AvailabilityModel.Response(doctor.Id,
            finalAvailabilities.Select(a => 
            new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour, a.EndingHour)));

    }
    private ICollection<AvailabilityRule> CreateFinalAvailabilities(
        IEnumerable<AvailabilityModel.DayScheduleRequest> days,
        Doctor doctor,
        int currentMonth, int currentYear,
        DateTime now)
    {
        ICollection<AvailabilityRule> createdAvailabilities = [];
        foreach (var day in days)
        {
            if (day.StartTime >= day.EndTime)
                throw new BusinessRuleException()
                    .WithDetail(nameof(day), Issue.DAY_ERROR);

            DayOfWeek weekDay = ParseDayOfWeek(day.Day);

            bool hasOverlap = doctor.AvailabilityRules.Any(a =>
            a.Month == currentMonth &&
            a.Year == currentYear &&
            a.WeekDay == weekDay &&
            a.HasOverlappingSchedules(day.StartTime, day.EndTime));

            if (hasOverlap)
                throw new ConflictException()
                    .WithDetail(nameof(hasOverlap), Issue.OVERLAP);

            var availabilityRule = new AvailabilityRule(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime, doctor);

            availabilityRule.GenerateMonthlyAvailabilitySlots(now.Day);

            createdAvailabilities.Add(availabilityRule);
        }
        return createdAvailabilities;
    }
    private static DayOfWeek ParseDayOfWeek(string day)
    {
        if (Enum.TryParse<DayOfWeek>(day, true, out var weekDay))
            return weekDay;

        return day.ToLower() switch
        {
            "domingo" => DayOfWeek.Sunday,
            "lunes" => DayOfWeek.Monday,
            "martes" => DayOfWeek.Tuesday,
            "miercoles" or "miércoles" => DayOfWeek.Wednesday,
            "jueves" => DayOfWeek.Thursday,
            "viernes" => DayOfWeek.Friday,
            "sabado" or "sábado" => DayOfWeek.Saturday,
            _ => throw new ValidationException().WithDetail(nameof(day), Issue.INVALID_DAY)
        };
    }

}
