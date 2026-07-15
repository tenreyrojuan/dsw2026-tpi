using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dsw2026Tpi.Domain.Entities;
//Disponibilidad
public class Availability : EntityBase
{
    public int Month { get; private set; } /*[Consultar]: estas 5 no deberian ser init, para que sea inmutable la disponibilidad a menos que se la borre*/
    public int Year { get; private set; }
    public int WeekDay { get; private set; }
    public TimeOnly StartingHour { get; private set; }
    public TimeOnly EndingHour { get; private set; }

    public Guid DoctorId { get; private set; }
    // Reference navigation (to the "one" side) one-to-many relationship
    public Doctor Doctor { get; private set; }

    // Collection navigation (to the "many" side): Cada disponibilidad puede tener varios turnos
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
