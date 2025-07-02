using CineSocial.Api.Extensions;
using CineSocial.Api.Middleware;
using CineSocial.Core.Extensions;
using CineSocial.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCore();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CineSocial API V1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

    // Try to seed database in development (skip if DB not available)
    try
    {
        await app.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetService<ILogger<Program>>();
        logger?.LogWarning(ex, "Database seeding skipped: {Message}", ex.Message);
    }
}
else if (!app.Environment.IsEnvironment("Testing"))
{
    // Only migrate in production (skip for testing environment)
    await app.MigrateDatabaseAsync();
}

app.UseHttpsRedirection();

// Add custom middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowSpecificOrigins");

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }