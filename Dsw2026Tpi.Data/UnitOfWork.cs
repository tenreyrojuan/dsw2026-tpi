using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly Dsw2026TpiDbContext _dbContext;
    public UnitOfWork(Dsw2026TpiDbContext context, Dsw2026TpiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}