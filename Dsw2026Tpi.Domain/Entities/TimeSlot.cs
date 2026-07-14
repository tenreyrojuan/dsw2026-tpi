using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Turno
public class TimeSlot : EntityBase
{
    public DateTime Date { get; init; }
    public DateTime StartingTime { get; init; }
    public DateTime EndingTime { get; init; }
    #region Constructor for EF
    private TimeSlot() { }
    #endregion
    public TimeSlotState TimeSlotState { get; private set; }
    public TimeSlot(DateTime date, DateTime startingTime,
                DateTime endingTime,Guid? id = null) : base(id)
    {
        Date = date;
        StartingTime = startingTime;
        EndingTime = endingTime;
        TimeSlotState = TimeSlotState.Available;
    }
    public void ChangeState(TimeSlotState newState)
    {
        TimeSlotState = newState;
    }
}
