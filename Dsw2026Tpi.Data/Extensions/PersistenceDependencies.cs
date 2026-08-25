using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Dsw2026Tpi.Data.Extensions;

public static class PersistenceDependencies
{
    public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IPersistence, PersistenceEf>();
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        return services;
    }
}