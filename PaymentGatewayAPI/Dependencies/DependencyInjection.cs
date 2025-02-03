using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.DatabaseContext;

namespace PaymentGatewayAPI.Dependencies;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDatabaseProvider(configuration);
        return services;
    }

    public static IServiceCollection AddDatabaseProvider(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }

}
