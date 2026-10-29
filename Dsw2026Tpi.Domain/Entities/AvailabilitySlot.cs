using Dsw2026Tpi.CrossCutting.Exceptions;

namespace Dsw2026Tpi.Domain.Entities;
//Turno
public sealed class AvailabilitySlot : EntityBase
{
    internal const int SlotDurationMinutes = 30;
    
    #region Constructor for EF
        #pragma warning disable CS8618
            private AvailabilitySlot() { }
        #pragma warning restore CS8618
            #endregion
    
    private AvailabilitySlot(
        DateOnly date,
        TimeOnly startingTime,
        TimeOnly endingTime,
        AvailabilityRule availabilityRule,
        Guid? id = null) 
        : base(id)
    {
        Date = date;
        StartingTime = startingTime;
        EndingTime = endingTime;
        AvailabilityRule = availabilityRule;
        AvailabilitySlotState = AvailabilitySlotState.AVAILABLE;
    }

    internal static AvailabilitySlot Create(
        DateOnly date,
        TimeOnly startingTime,
        TimeOnly endingTime, 
        AvailabilityRule availabilityRule)
    {
        if (date < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new BusinessRuleException()
                .WithDetail(
                    nameof(date),
                    "La fecha del turno no puede ser menor a la fecha actual");
        }

        if (startingTime >= endingTime)
        {
            throw new BusinessRuleException()
                .WithDetail(
                    $"{nameof(startingTime)} and {nameof(endingTime)}",
                    "El tiempo de comienzo no puede ser menor a la de final");
        }
        
        // los slots deben durar 30 minutos, por lo que nos aseguramos 
        // que al crear un slot, no podamos crear turnos menores o mayores
        // a 30 minutos
        var duration = endingTime - startingTime;
        if (duration != TimeSpan.FromMinutes(SlotDurationMinutes))
        {
            throw new BusinessRuleException()
                .WithDetail($"{nameof(startingTime)} and {nameof(endingTime)}",
                    "Los slots deben ser de 30 minutos");
        }
        
        return new AvailabilitySlot(date,startingTime, endingTime, availabilityRule);
    }
    
    public DateOnly Date { get; private set; }
    public TimeOnly StartingTime { get; private set; }
    public TimeOnly EndingTime { get; private set; }

    public AvailabilitySlotState AvailabilitySlotState { get; private set; }

    public Guid AvailabilityRuleId { get; private set; }

    public AvailabilityRule AvailabilityRule { get; private set; }

    public Appointment? Appointment { get; private set; }
    
    public bool IsAvailable =>  AvailabilitySlotState == AvailabilitySlotState.AVAILABLE;
    public bool IsBooked => AvailabilitySlotState == AvailabilitySlotState.BOOKED;
    

    public void Book(Appointment appointment)
    {
        if (!IsAvailable)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(AvailabilitySlotState), "El turno no esta disponible");
        }
        Appointment = appointment;
        AvailabilitySlotState = AvailabilitySlotState.BOOKED;
        UpdateTimestamp();
    }

    public void Release()
    {
        if (!IsBooked)
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(AvailabilitySlotState), "El turno no esta reservado");
        }
        AvailabilitySlotState = AvailabilitySlotState.AVAILABLE;
        UpdateTimestamp();
    }
}