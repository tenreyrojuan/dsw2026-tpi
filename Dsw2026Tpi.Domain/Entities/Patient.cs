using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;

public class Patient : EntityBase
{
    public string Dni { get; init; }
    public string Name { get; init; }
    public string Phone { get; private set; }

    // Collection Navigation (to the "many" side) one-to-many relationship, un 1 paciente gestiona N citas
    public ICollection<Appointment> Appointments { get; private set; } = [];

    #region Constructor for EF
#pragma warning disable CS8618
    private Patient()
    {

    }
#pragma warning restore CS8618
    #endregion
    public Patient(string dni, string name, string phone, Guid? id =  null): base(id)
    {
        Dni = dni;
        Name = name;
        Phone = phone;
    }
}
