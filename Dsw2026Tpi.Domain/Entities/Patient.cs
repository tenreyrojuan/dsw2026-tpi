namespace Dsw2026Tpi.Domain.Entities;

public sealed class Patient : EntityBase
{
    public string Dni { get; init; }
    public string FullName { get; init; }

    public Guid UserId { get; init; }

    public ICollection<Appointment> Appointments { get; private set; } = [];

    #region Constructor for EF
#pragma warning disable CS8618
    private Patient()
    {

    }
#pragma warning restore CS8618
    #endregion
    public Patient(string dni, string name, Guid? id =null) : base(id)
    {
        Dni = dni;
        FullName = name;
    }
}