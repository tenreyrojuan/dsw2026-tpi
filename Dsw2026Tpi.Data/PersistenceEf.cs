using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Dsw2026Tpi.Data.Specifications;

namespace Dsw2026Tpi.Data;

internal sealed class PersistenceEf : IPersistence
{
    private readonly Dsw2026TpiDbContext _context;

    public PersistenceEf(Dsw2026TpiDbContext context)
    {
        _context = context;
    }

    public T Add<T>(T entity) where T : EntityBase
    {
        _context.Add(entity);
        return entity;
    }

    public T Delete<T>(T entity) where T : EntityBase
    {
        _context.Remove(entity);
        return entity;
    }

    public async Task<T?> First<T>(Expression<Func<T,bool>> predicate,params string[] includes) where T : EntityBase
    {
        return await Include(_context.Set<T>(),includes).FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IEnumerable<T>?> GetAll<T>(ISpecification<T> spec) where T : EntityBase
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<T?> GetById<T>(Guid id, params string[] includes) where T : EntityBase
    {
        return await Include(_context.Set<T>(), includes).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<T>?> GetFiltered<T>(ISpecification<T> spec) where T : EntityBase
    {
        return await ApplySpecification(spec).ToListAsync();
    }
    /// <summary>
    /// Sirve para verificar la existe de alguna entidad sin traerla a la memoria.
    /// Surge de la necesidad de verificar la existencia de alguna entidad que no se va a usar mas adelante
    /// Nota: previamente se utilizaba GetById o First para verificar la existencia
    /// </summary>
    public async Task<bool> Any<T> (Expression<Func<T,bool>> predicate) where T : EntityBase
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }
    /// <summary>
    /// Metodo de añadir y eliminar colecciones 
    /// Surge de la necesidad de poder actualizar las colecciones de una entidad
    /// sin hacer ciclos de eliminacion costosos (debido a las llamadas constantes a SaveChanges).
    /// 
    /// Nota: cambiar las disponibilidades del medico y actualizar no funciona. 
    /// Al parecer EF no trackeaba los cambios de la coleccion
    /// </summary>

    public IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : EntityBase
    {
        _context.AddRange(entities);
        return entities;
    }
    public IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities) where T : EntityBase
    {
        _context.RemoveRange(entities);
        return entities;
    }
    public T Update<T>(T entity) where T : EntityBase
    {
        _context.Update(entity);
        return entity;
    }
    public async Task<Pagination<T>> Paginate<T, TKey>(int pageSize, int pageIndex,
        ISpecification<T> spec) where T : EntityBase
    {
        pageSize = Math.Abs(pageSize);
        pageIndex = Math.Abs(pageIndex) == 0 ? 0 : Math.Abs(pageIndex) - 1;

        var filtered = ApplySpecification(spec);

        var total = await filtered.CountAsync();

        async Task<Pagination<T>> GetPage(int skip, int take)
        {
            var data = await filtered.Skip(skip)
                    .Take(take)
                    .ToListAsync();

            return new Pagination<T>(pageSize, pageIndex, data, total);
        }

        //la pagina existe
        if (total > pageSize * pageIndex)
        {
            return await GetPage(pageIndex * pageSize, pageSize);
        }

        //solo hay una pagina
        if (total < pageSize)
        {
            return new Pagination<T>(pageSize, pageIndex, await filtered.ToListAsync(), total);
        }

        var targetPageIndex = pageIndex - 1;

        while (true)
        {
            if (total > targetPageIndex * pageSize)
            {
                return await GetPage(targetPageIndex * pageSize, pageSize);
            }

            targetPageIndex--;

            if (targetPageIndex < 0) return new Pagination<T>(pageSize, 0, [], 0);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private static IQueryable<T> Include<T>(IQueryable<T> query, string[] includes) where T : EntityBase
    {
        var includedQuery = query;

        foreach (var include in includes)
        {
            includedQuery = includedQuery.Include(include);
        }
        return includedQuery;
    }

    private IQueryable<T> ApplySpecification<T>(ISpecification<T> spec)  where T : EntityBase
    {
        return _context.Set<T>().GetQuery(spec);
    }
}
