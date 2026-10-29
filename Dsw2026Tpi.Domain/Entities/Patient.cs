namespace Dsw2026Tpi.Domain.Entities;

public sealed class Patient : EntityBase
{
    #region Constructor for EF
#pragma warning disable CS8618
    private Patient()
    {

    }
#pragma warning restore CS8618
    #endregion
    public Patient(string dni, string name, Guid userId, Guid? id =null) : base(id)
    {
        Dni = dni;
        FullName = name;
        UserId = userId;
    }
    
    public string Dni { get; private set; }
    public string FullName { get; private set; }

    public Guid UserId { get; private set; }

    private List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();
}