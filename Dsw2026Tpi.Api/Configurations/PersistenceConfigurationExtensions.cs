using System.Runtime.InteropServices;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Data.Extensions;
using Dsw2026Tpi.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Tpi.Api.Configurations;

public static class PersistenceConfigurationExtensions
{
    public static IServiceCollection AddApplicationPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Obtener cadena de conexión desde appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var alternateConnection = configuration.GetConnectionString("AlternateConnection");

        //Agregar contexto (O/RM) y utilizar SQL Server para DB
        
        services.AddDbContext<Dsw2026TpiDbContext>(options =>
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                options.UseSqlServer(connectionString);
            else
                options.UseNpgsql(alternateConnection);
        });

        services.AddDbContext<AuthenticationDbContext>(options =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseNpgsql(alternateConnection);
            }
            
            options.UseSeeding((c, t) =>
            {
                var rolesPath = Path.Combine(AppContext.BaseDirectory, "Sources", "roles.json");
                c.Seedwork<IdentityRole>(rolesPath);
            });
        });
        return services;
    }
}
