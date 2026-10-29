using Dsw2026Tpi.CrossCutting.Exceptions;

namespace Dsw2026Tpi.Domain.Entities;
//Disponibilidad
public sealed class AvailabilityRule : EntityBase
{
    private readonly List<AvailabilitySlot> _availabilitySlots = new();

    #region Constructor for EF
#pragma warning disable CS8618
    private AvailabilityRule()
    {
    }
#pragma warning restore CS8618
    #endregion
    
    private AvailabilityRule(
        int month, 
        int year, 
        DayOfWeek weekDay,
        TimeOnly startingHour,
        TimeOnly endingHour, 
        Doctor doctor, 
        Guid? id = null) 
        : base(id)
    {
        Month = month;
        Year = year;
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
        Doctor = doctor;
    }
    
    internal static AvailabilityRule Create(
        int month, 
        int year, 
        DayOfWeek weekDay, 
        TimeOnly startingHour,
        TimeOnly endingHour, 
        Doctor doctor)
    {
        if (month < 1 || month > 12)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(month), "Mes invalido.");
        }

        if (year < DateTime.Now.Year)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(year), "Año invalido.");
        }
        if (startingHour >= endingHour)
        {
            throw new BusinessRuleException()
                .WithDetail(
                    $"{nameof(startingHour)} and {nameof(endingHour)}",
                    "El tiempo de comienzo no puede ser menor a la de final");
        }
        
        return new AvailabilityRule(month, year, weekDay, startingHour, endingHour, doctor);
    }
    
    public int Month { get; private set; }
    public int Year { get; private set; }

    public DayOfWeek WeekDay { get; private set; }
    public TimeOnly StartingHour { get; private set; }
    public TimeOnly EndingHour { get; private set; }

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; }
    
    public IReadOnlyCollection<AvailabilitySlot> AvailabilitySlots => _availabilitySlots.AsReadOnly();

    public bool IsOfMonth(int currentMonth, int currentYear) => Month == currentMonth && Year == currentYear;
    
    public bool HasBookedSlots => _availabilitySlots.Any(s => s.IsBooked);
    

    public bool HasOverlappingSchedules(TimeOnly requestedStart, TimeOnly requestedEnd)
    {
        return requestedStart < EndingHour && StartingHour < requestedEnd;
    }

    public void GenerateMonthlyAvailabilitySlots(int startingDay, IEnumerable<DateOnly> holidays)
    {
        var date = new DateOnly(Year, Month, startingDay);
        //busca el primer dia del mes que coincida con el dia de semana presente en el array del request
        while (date.DayOfWeek != WeekDay)
        {
            date = date.AddDays(1);
            if (date.Month != Month) return;
        }
        //crea los turnos diarios para el dia solicitado, saltando de a 7 dias hasta que termina el mes.
        while (date.Month == Month)
        {
            if (!holidays.Contains(date))
            {
                CreateDailyAvailabilitySlots(date);
            }
            date = date.AddDays(7);
        }
    }

    private void CreateDailyAvailabilitySlots(DateOnly date)
    {
        var currentStart = StartingHour;

        while (currentStart < EndingHour)
        {
            var currentEnd = currentStart.AddMinutes(AvailabilitySlot.SlotDurationMinutes);

            if (currentEnd > EndingHour)
                break;

            _availabilitySlots.Add(AvailabilitySlot.Create(date, currentStart, currentEnd, this));

            currentStart = currentEnd;
        }
    }
}