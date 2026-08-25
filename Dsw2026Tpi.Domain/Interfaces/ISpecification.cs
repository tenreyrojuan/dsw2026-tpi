using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Interfaces;

public interface ISpecification<T> where T : EntityBase
{
    Expression<Func<T, bool>>? Criteria { get; }
    ICollection<string>? Includes { get; }
    Expression<Func<T, object>>? OrderByExpression { get;}
    Expression<Func<T, object>>? OrderByDescendingExpression { get;}
    bool AsNoTracking { get; }
    bool AsSplitQuery { get; }
}   