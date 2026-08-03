namespace Dsw2026Tpi.Domain.Entities;
//Cita
public sealed class Appointment : EntityBase
{
    public string Reason { get; private set; }

    public DateTime? AttendedAt { get; private set; }
    public DateTime? CancelledAt{ get; private set; }
    public AppointmentState AppointmentState { get; private set; }
    public Guid PatientId { get; init; }
    public Guid AvailabilitySlotId { get; init; }
    public Patient Patient { get; init; }
    public AvailabilitySlot AvailabilitySlot { get; init; }

    #region Constructor for EF
#pragma warning disable CS8618
    private Appointment()
    {
    }
#pragma warning restore CS8618
    #endregion
    public Appointment (Patient patient,
                AvailabilitySlot availabilitySlot, string reason, Guid? id = null) : base(id)
    {
        Patient = patient;
        AvailabilitySlot = availabilitySlot;
        Reason = reason;
        AppointmentState = AppointmentState.BOOKED;
    }

    public void Cancel()
    {
        AppointmentState = AppointmentState.CANCELLED;
        CancelledAt = DateTime.Now;
        AvailabilitySlot.Release();
        UpdateTimestamp();
    }
    
    public void Complete()
    {
        AppointmentState = AppointmentState.ATTENDED;
        AttendedAt = DateTime.Now;
        UpdateTimestamp();
    }
}
