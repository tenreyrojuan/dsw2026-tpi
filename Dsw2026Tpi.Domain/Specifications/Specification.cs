using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Domain.Specifications;

public abstract class Specification<T> : ISpecification<T> where T : EntityBase
{
    protected Specification(Expression<Func<T, bool>>? criteria, params string[] includes)
    {
        Criteria = criteria;
        Includes = includes?.ToList() ?? [];
    }
    public Expression<Func<T, bool>>? Criteria { get; }
    public ICollection<string>? Includes { get; private set; }
    public Expression<Func<T, object>>? OrderByExpression { get; private set; }
    public Expression<Func<T, object>>? OrderByDescendingExpression { get; private set; }
    public bool AsNoTracking { get; private set; }
    public bool AsSplitQuery { get; private set; }


    protected void AddInclude(string include)
    {
        Includes?.Add(include);
    }
    
    protected void AddOrderBy(Expression<Func<T, object>> orderBy)
    {
        OrderByExpression = orderBy;
    }
    
    protected void AddOrderByDescending(Expression<Func<T, object>> orderBy)
    {
        OrderByDescendingExpression = orderBy;
    }

    protected void SetAsNoTracking()
    {
        AsNoTracking = true;
    }
    
    protected void SetAsSplitQuery()
    {
        AsSplitQuery = true;
    }
}