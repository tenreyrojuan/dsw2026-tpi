using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Cita
public class Appointment : EntityBase
{
    public DateTime ServiceDate { get; init; }
    public DateTime CancelationDate { get; init; }
    public AppointmentState AppointmentState { get; private set; }
    public Guid PatientId { get; init; }
    public Patient Patient { get; private set; }
    public Guid TimeSlotId { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    #region Constructor for EF
    private Appointment()
    {
    }
    #endregion
    public Appointment (DateTime serviceDate, DateTime cancelationDate,
                AppointmentState appointmentState,Patient patient,
                TimeSlot timeSlot, Guid? id = null) : base(id)
    {
        ServiceDate = serviceDate;
        CancelationDate = cancelationDate;
        AppointmentState = appointmentState;
        Patient = patient;
        TimeSlot = timeSlot;
    }
}
