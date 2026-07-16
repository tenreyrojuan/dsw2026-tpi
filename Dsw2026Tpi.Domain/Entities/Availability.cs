using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Disponibilidad
public class Availability : EntityBase
{
    public int Month { get; init; } 
    public int Year { get; init; }
    public int WeekDay { get; init; }
    public TimeOnly StartingHour { get; init; }
    public TimeOnly EndingHour { get; init; }

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
    public Availability(int month, int year, int weekDay, TimeOnly startingHour,
        TimeOnly endingHour, Doctor doctor, Guid? id = null) : base(id)
    {
        Month = month;
        Year = year;
        WeekDay = weekDay;
        StartingHour = startingHour;
        EndingHour = endingHour;
        Doctor = doctor;
        DoctorId = doctor.Id;
    }

    public void AddTimeSlot(TimeSlot timeSlot)
    {
        TimeSlots.Add(timeSlot);
    }
}
