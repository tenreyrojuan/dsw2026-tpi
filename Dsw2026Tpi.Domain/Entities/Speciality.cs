namespace Dsw2026Tpi.Domain.Entities;

public class Speciality: EntityBase
{
    public string Name { get; init; } /* [Consultar] : no es mejor private set? pues con init es inmutable y no colabora con el CRUD completo (fallaria en el PUT)*/
    public string Description { get; init; }

    #region Constructor for EF
#pragma warning disable CS8618
    private Speciality() { }
#pragma warning restore CS8618
    #endregion

    public Speciality(string name, string description, Guid? id = null) : base(id)
    {
        Name = name;
        Description = description;
    }
}
