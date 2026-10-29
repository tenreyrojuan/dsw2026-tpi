using System.Collections.Immutable;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.CrossCutting.Helpers;

namespace Dsw2026Tpi.Domain.Entities;

public sealed class Doctor : EntityBase
{
    private readonly List<AvailabilityRule> _availabilityRules = new();
    
    private Doctor(string name, string licenseNumber, Specialty specialty, Guid? id = null) : base(id)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        Specialty = specialty;
    }

    public static Doctor Create(string name, string licenseNumber, Specialty specialty)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (name.Length < 3 || name.Length > 100)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(name),"El nombre debe tener entre 3 y 100 caracteres");
        }
        
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseNumber);

        return new Doctor(name, licenseNumber, specialty);
    }
    
    #region Constructor for EF
    #pragma warning disable CS8618
        private Doctor()
        {
        }
    #pragma warning restore CS8618
        #endregion
    
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public Guid SpecialtyId { get; private set; }
    public Specialty Specialty { get; private set; }

    public IReadOnlyCollection<AvailabilityRule> AvailabilityRules => _availabilityRules.AsReadOnly();

    public bool IsActive { get; private set; } = true;
    
    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void UpdateDoctor(string name, string licenseNumber, Guid specialtyId)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        SpecialtyId = specialtyId;
        UpdateTimestamp();
    }

    public AvailabilityRule AddAvailabilityRule(
        int month, 
        int year, 
        DayOfWeek weekDay,
        TimeOnly startingHour,
        TimeOnly endingHour)
    {
        return AvailabilityRule.Create(month, year, weekDay, startingHour, endingHour, this);
    }
    
    public IReadOnlyCollection<AvailabilityRule> CreateFinalAvailabilities(
        IReadOnlyCollection<(string Day, TimeOnly StartTime, TimeOnly EndTime)> days,
        IReadOnlyCollection<DateOnly> holidays,
        int currentMonth,
        int currentYear,
        DateTime now)
    {
        
        List<AvailabilityRule> createdAvailabilities = new();
        foreach (var day in days)
        {
            var weekDay = day.Day.ParseDayOfWeek();

            CheckOverlappingAvailabilityRules(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime);

            var availabilityRule 
                = AvailabilityRule.Create(currentMonth, currentYear, weekDay, day.StartTime, day.EndTime, this);

            availabilityRule.GenerateMonthlyAvailabilitySlots(now.Day, holidays);

            createdAvailabilities.Add(availabilityRule);
        }
        return createdAvailabilities.AsReadOnly();
    }

    private void CheckOverlappingAvailabilityRules(
        int currentMonth, 
        int currentYear, 
        DayOfWeek weekDay, 
        TimeOnly startTime,
        TimeOnly endTime)
    {
        bool hasOverlap = _availabilityRules.Any(
            a => a.Month == currentMonth &&
                 a.Year == currentYear &&
                 a.WeekDay == weekDay &&
                 a.HasOverlappingSchedules(startTime, endTime)
                 );
        
        if (hasOverlap) throw new ConflictException().WithDetail(nameof(hasOverlap), Issue.OVERLAP);
    }

    // se listan las disponibilidades que son de este mes, este año y ademas no tienen slots ocupados
    public List<AvailabilityRule> GetAvailableMonthlyAvailabilityRules(int currentMonth, int currentYear)
    {
        return AvailabilityRules
            .Where(rule =>
                rule.IsOfMonth(currentMonth, currentYear)
                && !rule.HasBookedSlots
            )
            .ToList();
    } 
}
