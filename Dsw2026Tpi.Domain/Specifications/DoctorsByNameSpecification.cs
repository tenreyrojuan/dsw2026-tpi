using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public sealed class DoctorsByNameSpecification : Specification<Doctor>
{
    public DoctorsByNameSpecification(string? name, params string[] includes) : 
        base(doctor => doctor.IsActive == true && (string.IsNullOrWhiteSpace(name) || doctor.Name.Contains(name)))
    {
        foreach (var include in includes)
        {
            AddInclude(include);
        }
        AddOrderBy(d => d.Name);
        SetAsNoTracking();
    }
}