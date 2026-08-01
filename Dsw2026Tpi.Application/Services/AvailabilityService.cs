using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

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
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId, nameof(Doctor.Availabilities))
            ?? throw new EntityNotFoundException(nameof(Doctor));

        var now = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        ICollection<Availability> finalAvailabilities = [];
        foreach (var day in request.Days)
        {
            if (day.StartTime >= day.EndTime)
                throw new ConflictException(ErrorCodes.BUSINESS_ERROR, "startTime debe ser antes que endTime!");

            DayOfWeek weekDay = ParseDayOfWeek(day.Day);

            bool hasOverlap = doctor.Availabilities.Any(a =>
            a.Month == currentMonth &&
            a.Year == currentYear &&
            a.WeekDay == weekDay &&
            day.StartTime < a.EndingHour &&
            day.EndTime > a.StartingHour);

            if (hasOverlap)
                throw new ConflictException(ErrorCodes.BUSINESS_ERROR, "se ha detectado un solapamiento de horarios");

            var availability = new Availability(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime, doctor);

            availability.GenerateMonthlyTimeSlots(now.Day);

            doctor.Availabilities.Add(availability);
            finalAvailabilities.Add(availability);
        }
        var updatedDoctor = await _persistence.Update(doctor);

        return new AvailabilityModel.Response(updatedDoctor.Id,
            finalAvailabilities.Select(a => new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour, a.EndingHour)));
    }

    public async Task<AvailabilityModel.Response> UpdateAvailability(AvailabilityModel.Request request)
    {
        Doctor? doctor = await _persistence.GetById<Doctor>(request.DoctorId, nameof(Doctor.Availabilities))
            ?? throw new EntityNotFoundException(nameof(Doctor));

        var now = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        var availabilitiesToKeep = doctor.Availabilities
            .Where(a => a.Month != currentMonth || a.Year != currentYear)
            .ToArray();

        doctor.UpdateDoctorAvailabilities(availabilitiesToKeep);

        ICollection<Availability> finalAvailabilities = [];
        foreach (var day in request.Days)
        {
            if (day.StartTime >= day.EndTime)
                throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, "startTime debe ser antes que endTime!");

            DayOfWeek weekDay = ParseDayOfWeek(day.Day);

            bool hasOverlap = doctor.Availabilities.Any(a =>
            a.Month == currentMonth &&
            a.Year == currentYear &&
            a.WeekDay == weekDay &&
            a.HasOverlappingSchedules(day.StartTime, day.EndTime));

            if (hasOverlap)
                throw new ConflictException(ErrorCodes.BUSINESS_ERROR, "se ha detectado un solapamiento de horarios");

            var availability = new Availability(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime, doctor);

            availability.GenerateMonthlyTimeSlots(now.Day);

            doctor.Availabilities.Add(availability);
            finalAvailabilities.Add(availability);
        }
        var updatedDoctor = await _persistence.Update(doctor);

        return new AvailabilityModel.Response(updatedDoctor.Id,
            finalAvailabilities.Select(a => new AvailabilityModel.DayScheduleRequest(a.WeekDay.ToString(), a.StartingHour, a.EndingHour)));

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
            _ => throw new ArgumentException($"Día inválido: {day}")
        };
    }

}
