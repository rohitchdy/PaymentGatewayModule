using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Common;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Services;

namespace PaymentGatewayAPI.Dependencies;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDatabaseProvider(configuration);
        services.AddPasswordHash(configuration);
        services.AddScoped<ISeedService, SeedService>();
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

    public static IServiceCollection AddPasswordHash(this IServiceCollection services, ConfigurationManager configuration)
    {
        var passwordHashSettings = new PasswordHashSettings();
        configuration.Bind(PasswordHashSettings.SectionName, passwordHashSettings);
        services.AddSingleton(Options.Create(passwordHashSettings));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }

}
