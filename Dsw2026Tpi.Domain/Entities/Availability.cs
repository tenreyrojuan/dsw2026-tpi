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

    public void AddTimeSlot(TimeSlot timeSlot)
    {
        TimeSlots.Add(timeSlot);
    }

    public void UpdateAvailability(DayOfWeek weekDay, TimeOnly startingHour, TimeOnly endingHour, Guid doctorId)
    {
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
    }
}
