namespace Dsw2026Tpi.Domain.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}