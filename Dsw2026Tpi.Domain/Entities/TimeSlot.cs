namespace Dsw2026Tpi.Domain.Entities;
//Turno
public class TimeSlot : EntityBase
{
    public DateOnly Date { get; private set; }
    public TimeOnly StartingTime { get; private set; }
    public TimeOnly EndingTime { get; private set; }

    public TimeSlotState TimeSlotState { get; private set; }

    public Guid AvailabilityId { get; private set; }

    public Availability Availability { get; private set; }

    public Appointment? Appointment { get; private set; }

    public const int SlotDurationMinutes = 30;

    #region Constructor for EF
#pragma warning disable CS8618
    private TimeSlot() { }
#pragma warning restore CS8618
    #endregion

    public TimeSlot(DateOnly date, TimeOnly startingTime,
                TimeOnly endingTime, Availability availability, Guid? id = null) : base(id)
    {
        Date = date;
        StartingTime = startingTime;
        EndingTime = endingTime;
        Availability = availability;
        TimeSlotState = TimeSlotState.AVAILABLE;
    }
    public void ChangeState(TimeSlotState newState)
    {
        TimeSlotState = newState;
    }
    public void Book(Appointment appointment)
    {
        Appointment = appointment;
        TimeSlotState = TimeSlotState.BOOKED;
    }
}