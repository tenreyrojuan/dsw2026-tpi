using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;

namespace Dsw2026Tpi.Domain.Entities;
//Cita
public sealed class Appointment : EntityBase
{
    
    #region Constructor for EF
#pragma warning disable CS8618
    private Appointment()
    {
    }
#pragma warning restore CS8618
    #endregion
    public Appointment (Patient patient,
        AvailabilitySlot availabilitySlot,
        string reason,
        Guid? id = null) 
        : base(id)
    {
        Patient = patient;
        AvailabilitySlot = availabilitySlot;
        Reason = reason;
        AppointmentState = AppointmentState.BOOKED;
    }

    public static Appointment Create(
        Patient patient,
        AvailabilitySlot availabilitySlot,
        string reason)
    {
        if (!reason.IsReasonValid())
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(reason), "La razon debe tener al menos 5 caracteres");
        }
        
        return new Appointment(patient, availabilitySlot, reason);
    }
    
    public string Reason { get; private set; }

    public DateTime? AttendedAt { get; private set; }
    public DateTime? CancelledAt{ get; private set; }
    public AppointmentState AppointmentState { get; private set; }
    public Guid PatientId { get; init; }
    public Guid AvailabilitySlotId { get; init; }
    public Patient Patient { get; init; }
    public AvailabilitySlot AvailabilitySlot { get; init; }
    
    public bool IsBooked => AppointmentState == AppointmentState.BOOKED;
    public bool IsCanceled => AppointmentState == AppointmentState.CANCELLED;
    public bool IsAttended => AppointmentState == AppointmentState.ATTENDED;

    public void Cancel()
    {
        if (!IsBooked)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(AppointmentState), "El turno no esta reservado");
        }
        
        AppointmentState = AppointmentState.CANCELLED;
        CancelledAt = DateTime.Now;
        
        AvailabilitySlot.Release();
        UpdateTimestamp();
    }
    
    public void Complete()
    {
        if (!IsBooked)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(AppointmentState), "El turno no esta reservado");
        }
        
        AppointmentState = AppointmentState.ATTENDED;
        AttendedAt = DateTime.Now;
        UpdateTimestamp();
    }
}
