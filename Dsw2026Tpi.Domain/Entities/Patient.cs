using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;

public class Patient : EntityBase
{
    public string DNI { get; init; }
    public string Name {  get; init; }
    public string Phone { get; private set; }
    #region Constructor for EF
    private Patient()
    {

    }
    #endregion
    public Patient(string dni,string name, string phone,Guid? id =  null): base(id)
    {
        DNI = dni;
        Name = name;
        Phone = phone;
    }
}
