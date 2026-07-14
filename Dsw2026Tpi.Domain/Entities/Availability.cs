using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Disponibilidad
public class Availability : EntityBase
{
    public int Month { get; private set; }
    public int Year { get; private set; }
    public int WeekDay { get; private set; }
    public TimeOnly StartingHour { get; private set; }
    public TimeOnly EndingHour { get; private set; }
    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; }

    public Guid TimeSlotId { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    #region Constructor for EF
    private Availability()
    {
    }
    #endregion
    public Availability(int month, int year, int weekDay, TimeOnly startingHour,
        TimeOnly endingHour, Doctor doctor, TimeSlot timeSlot, Guid? id = null) : base(id)
    {
        Month = month;
        Year = year;
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
        Doctor = doctor;
        TimeSlot = timeSlot;
    }
}
