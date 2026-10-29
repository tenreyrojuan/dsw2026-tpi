using System.ComponentModel;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;

namespace Dsw2026Tpi.Domain.Entities;

public sealed class Specialty : EntityBase
{
    
    #region Constructor for EF
#pragma warning disable CS8618
    private Specialty() { }
#pragma warning restore CS8618
    #endregion

    public Specialty(string name, string description, Guid? id = null) : base(id)
    {
        Name = name;
        Description = description;
    }
    public static Specialty Create(string name, string description)
    {
        if (!name.IsNameValid())
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(name), "El nombre debe tener entre 3 y 100 caracteres");
        }
        if (!description.IsNameValid())
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(description), "La description debe tener entre 10 y 100 caracteres");
        }
        
        return new Specialty(name, description);
    }
    
    public string Name { get; private set; }
    public string Description { get; private set; }

    //metodo nuevo acutalizar especialidades
    public void UpdateSpecialty(string name, string description)
    {
        if (!name.IsNameValid())
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(name), "El nombre debe tener entre 3 y 100 caracteres");
        }
        if (!description.IsNameValid())
        {
            throw new BusinessRuleException()
                .WithDetail(nameof(description), "La description debe tener entre 10 y 100 caracteres");
        }
        
        Name = name;
        Description = description;
        UpdateTimestamp();
    }
}
