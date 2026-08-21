using Dsw2026Tpi.Api.Services;
using Dsw2026Tpi.Application.Extensions;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Data.Extensions;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Api.Configurations;

public static class DependencyInjectionConfigurationExtensions
{
    public static IServiceCollection AddAppDependencies(this IServiceCollection services)
    {
        services.AddPersistenceDependencies();
        services.AddApplicationDependencies();
        services.AddScoped<ISignInService, SignInService>();
        return services;
    }
}
