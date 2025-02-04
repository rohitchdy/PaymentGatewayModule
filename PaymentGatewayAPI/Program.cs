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
    }); ;


    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<RetryMiddleware>();
    app.MapControllers();

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
