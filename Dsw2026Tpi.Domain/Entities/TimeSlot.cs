using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Turno (debil respecto a la disponibilidad)
public class TimeSlot : EntityBase
{
    public DateOnly Date { get; init; }
    public TimeOnly StartingTime { get; init; }
    public TimeOnly EndingTime { get; init; }

    public TimeSlotState TimeSlotState { get; private set; }

    // Reference navigation (to "one" side) 1 disponibilidad -> N turnos
    public Availability Availability { get; private set; }

    // relacion: puede o no haber una cita asignada al turno
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
