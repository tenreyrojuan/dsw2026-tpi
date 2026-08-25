using Dsw2026Tpi.Domain.Entities;
using System.Linq.Expressions;

namespace Dsw2026Tpi.Domain.Interfaces;

public interface IPersistence
{
    IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : EntityBase;
    IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities) where T : EntityBase;
    Task<bool> Any<T>(Expression<Func<T, bool>> predicate) where T : EntityBase;
    Task<T?> GetById<T>(Guid id, params string[] includes) where T : EntityBase;
    Task<IEnumerable<T>?> GetAll<T>(ISpecification<T> spec) where T : EntityBase;
    Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] includes) where T : EntityBase;
    Task<IEnumerable<T>?> GetFiltered<T>(ISpecification<T> spec) where T : EntityBase;
    T Add<T>(T entity) where T : EntityBase;
    T Update<T>(T entity) where T : EntityBase;
    T Delete<T>(T entity) where T : EntityBase;
    Task<Pagination<T>> Paginate<T, TKey>(int pageSize, int pageIndex, ISpecification<T> specs) where T : EntityBase;
    Task<int> SaveChangesAsync();
}
