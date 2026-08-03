namespace Dsw2026Tpi.Domain.Entities;
//Turno
public sealed class AvailabilitySlot : EntityBase
{
    public DateOnly Date { get; private set; }
    public TimeOnly StartingTime { get; private set; }
    public TimeOnly EndingTime { get; private set; }

    public AvailabilitySlotState AvailabilitySlotState { get; private set; }

    public Guid AvailabilityId { get; private set; }

    public AvailabilityRule AvailabilityRule { get; private set; }

    public Appointment? Appointment { get; private set; }

    public const int SlotDurationMinutes = 30;

    #region Constructor for EF
#pragma warning disable CS8618
    private AvailabilitySlot() { }
#pragma warning restore CS8618
    #endregion

    public AvailabilitySlot(DateOnly date, TimeOnly startingTime,
                TimeOnly endingTime, AvailabilityRule availabilityRule, Guid? id = null) : base(id)
    {
        Date = date;
        StartingTime = startingTime;
        EndingTime = endingTime;
        AvailabilityRule = availabilityRule;
        AvailabilitySlotState = AvailabilitySlotState.AVAILABLE;
    }

    public void Book(Appointment appointment)
    {
        Appointment = appointment;
        AvailabilitySlotState = AvailabilitySlotState.BOOKED;
        UpdateTimestamp();
    }

    public void Release()
    {
        AvailabilitySlotState = AvailabilitySlotState.AVAILABLE;
        UpdateTimestamp();
    }
}