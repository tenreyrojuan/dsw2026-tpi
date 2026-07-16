using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Turno
public class TimeSlot : EntityBase
{
    public DateOnly Date { get; init; }
    public TimeOnly StartingTime { get; init; }
    public TimeOnly EndingTime { get; init; }

    public TimeSlotState TimeSlotState { get; private set; }

    public Guid AvailabilityId { get; private set; }

    public Availability Availability { get; private set; }

    public Appointment? Appointment { get; private set; } 

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
        AvailabilityId = availability.Id;
        Availability = availability;
        TimeSlotState = TimeSlotState.Available;
    }
    public void ChangeState(TimeSlotState newState)
    {
        TimeSlotState = newState;
    }
    public void Book(Appointment appointment)
    {
        Appointment = appointment;
        TimeSlotState = TimeSlotState.Booked;
    }
}
