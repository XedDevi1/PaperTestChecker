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
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseDefaultPipeline();

app.MapGet("/healthz", () => Results.Json(new { status = "ok" }))
   .WithName("Health");

app.MapGet("/api/ping", () => Results.Json(new { pong = true }))
   .RequireAuthorization()
   .WithName("Ping");

app.Run();