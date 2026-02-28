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

app.UseDefaultPipeline();

app.MapGet("/healthz", () => Results.Json(new { status = "ok" }))
   .WithName("Health");

app.MapGet("/api/ping", () => Results.Json(new { pong = true }))
   .RequireAuthorization()
   .WithName("Ping");

app.Run();