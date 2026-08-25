using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public sealed class SpecialtiesByNameSpecification : Specification<Specialty>
{
    public SpecialtiesByNameSpecification(string? name,params string[] includes) 
        : base(e => string.IsNullOrWhiteSpace(name) || e.Name.Contains(name))
    {
        foreach (var include in includes)
        {
            AddInclude(include);
        }
        AddOrderBy(x => x.Name);
        SetAsNoTracking();
    }
}