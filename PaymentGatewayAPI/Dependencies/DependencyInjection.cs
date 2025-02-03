using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
        services.AddSwaggerConfiguration();
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

    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PaymentGatewayApis",
                Version = "v1",
                Description = "To test API from Swagger",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Url = new Uri("https://www.api.com/support"),
                    Email = "rohitchy165@gmail.com"
                },
                TermsOfService = new Uri("https://www.api.com/termsandservices"),
            });

            config.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
        });

        services.AddSwaggerGenNewtonsoftSupport();
    }

}
