using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentGatewayAPI.Common;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Services;
using System.Text;

namespace PaymentGatewayAPI.Dependencies;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDatabaseProvider(configuration);
        services.AddPasswordHash(configuration);
        services.AddScoped<ISeedService, SeedService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddAuth(configuration);
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

    public static IServiceCollection AddAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        });
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }

}
