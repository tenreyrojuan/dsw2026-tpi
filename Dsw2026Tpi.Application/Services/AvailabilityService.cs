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

        var now = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        var existingAvailabilities = (await _persistence.GetFiltered<Availability>(
                a => a.DoctorId == request.DoctorId && a.Month == currentMonth && a.Year == currentYear))?.ToList() ?? new List<Availability>();

        if (isUpdate)
        {
            foreach (var existing in existingAvailabilities)
            {
                await _persistence.Delete(existing);
            }
            existingAvailabilities.Clear();
        }

        foreach (var day in request.Days)
        {
            if (day.StartTime >= day.EndTime)
                throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, "startTime debe ser antes que endTime!");

            var weekDay = Enum.Parse<DayOfWeek>(day.Day, true); // se reciben los dias en ingles, el true indica que es case insensitive

            bool hasOverlap = existingAvailabilities.Any(a =>
            a.WeekDay == weekDay &&
            day.StartTime < a.EndingHour &&
            day.EndTime > a.StartingHour);

            if (hasOverlap)
                throw new BusinessRuleException(ErrorCodes.BUSINESS_ERROR, "se ha detectado un solapamiento de horarios");

            var availability = new Availability(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime, doctor);

            availability.GenerateMonthlyTimeSlots(now.Day);
            await _persistence.Add(availability);

            existingAvailabilities.Add(availability);
        }
    }
}
