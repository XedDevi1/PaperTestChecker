using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCorsPolicy(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddDatabase(builder.Configuration)
    .AddAiServices(builder.Configuration)
    .AddSwaggerWithAuth()
    .AddControllers();

builder.Services.AddApplicationServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaperTestChecker.Data.AppDbContext>();
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseDefaultPipeline();

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
