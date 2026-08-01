namespace Dsw2026Tpi.Domain.Entities;
//Cita
public sealed class Appointment : EntityBase
{
    public string Reason { get; private set; }

    public DateTime? AttendedAt { get; private set; }
    public DateTime? CancelledAt{ get; private set; }
    public AppointmentState AppointmentState { get; private set; }
    public Guid PatientId { get; init; }
    public Guid TimeSlotId { get; init; }
    public Patient Patient { get; private set; }
    public TimeSlot TimeSlot { get; private set; }

    #region Constructor for EF
#pragma warning disable CS8618
    private Appointment()
    {
    }
#pragma warning restore CS8618
    #endregion
    public Appointment (Patient patient,
                TimeSlot timeSlot, string reason, Guid? id = null) : base(id)
    {
        Patient = patient;
        TimeSlot = timeSlot;
        Reason = reason;
        AppointmentState = AppointmentState.BOOKED;
    }

    public void Cancel()
    {
        AppointmentState = AppointmentState.CANCELED;
        CancelledAt = DateTime.Now;
    }
    
    public void Complete()
    {
        AppointmentState = AppointmentState.ATTENDED;
        AttendedAt = DateTime.Now;
    }
}
