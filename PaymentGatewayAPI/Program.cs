using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Dependencies;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Middlewares;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddService(builder.Configuration);

    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

    //builder.Services.AddSpaStaticFiles(configuration =>
    //{
    //    configuration.RootPath = "wwwroot/PaymentApp/dist/payment-app/browser";
    //});

    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "PaymentApp/dist";
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularClient",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200") // Adjust if needed
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // Needed if using cookies or authentication
            });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Log.Information("Application Started.");
    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseSpaStaticFiles();
    }
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<RetryMiddleware>();
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapControllers();
    });
    app.UseSpaStaticFiles();

    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";
        if (builder.Environment.IsDevelopment())
        {
            spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        }
    });
    var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dataContext.Database.MigrateAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seedService.SeedAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}
