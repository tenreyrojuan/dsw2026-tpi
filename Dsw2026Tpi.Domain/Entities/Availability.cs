namespace Dsw2026Tpi.Domain.Entities;
//Disponibilidad
public class Availability : EntityBase
{
    public int Month { get; init; }
    public int Year { get; init; }

    public DayOfWeek WeekDay { get; private set; }
    public TimeOnly StartingHour { get; private set; }
    public TimeOnly EndingHour { get; private set; }

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; }

    public ICollection<TimeSlot> TimeSlots { get; private set; } = [];

    #region Constructor for EF
#pragma warning disable CS8618
    private Availability()
    {
    }
#pragma warning restore CS8618
    #endregion
    public Availability(int month, int year, DayOfWeek weekDay, TimeOnly startingHour,
        TimeOnly endingHour, Doctor doctor, Guid? id = null) : base(id)
    {
        Month = month;
        Year = year;
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
        Doctor = doctor;
    }

    public void UpdateAvailability(DayOfWeek weekDay, TimeOnly startingHour, TimeOnly endingHour, Guid doctorId)
    {
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
    }

    public bool HasOverlappingSchedules(TimeOnly startingHour, TimeOnly endingHour)
    {
        return startingHour < EndingHour && StartingHour < endingHour;
    }

    public void GenerateMonthlyTimeSlots(int startingDay) 
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
            CreateDailyTimeSlots(date);
            date = date.AddDays(7);
        }
    }

    private void CreateDailyTimeSlots(DateOnly date)
    {
        var currentStart = StartingHour;

        while (currentStart < EndingHour)
        {
            var currentEnd = currentStart.AddMinutes(TimeSlot.SlotDurationMinutes);

            if (currentEnd > EndingHour)
                break;

            TimeSlots.Add(new TimeSlot(date, currentStart, currentEnd, this));

            currentStart = currentEnd;
        }
    }
}
