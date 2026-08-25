using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Tpi.Data.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(this IQueryable<T> inputQuery, ISpecification<T> specification) where T : EntityBase
    {
        IQueryable<T> queryable = inputQuery;

        if (specification.Criteria != null)
            queryable = queryable.Where(specification.Criteria);

        if(specification.Includes != null)
            foreach (var include in specification.Includes)
                queryable = queryable.Include(include);

        if (specification.OrderByExpression != null)
            queryable = queryable.OrderBy(specification.OrderByExpression);

        if (specification.OrderByDescendingExpression != null)
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        
        if(specification.AsNoTracking is true)
            queryable = queryable.AsNoTracking();
        
        if(specification.AsSplitQuery is true)
            queryable = queryable.AsSplitQuery();
        
        return queryable;
    }
}