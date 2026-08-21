using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dsw2026Tpi.Application.Extensions;

public static class ApplicationDependencies
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAdvancedSearchesService, AdvancedSearchesService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddSingleton<JwtService>();
        return services;
    }
}