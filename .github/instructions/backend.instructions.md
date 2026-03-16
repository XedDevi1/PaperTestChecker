---
applyTo: backend/**/*.cs
---

# Backend Coding Instructions (ASP.NET Core 8 / C#)

## Architecture

- **Controllers are thin.** They validate the HTTP request, call a service, and return an `ActionResult<T>`. No business logic in controllers.
- **Services hold the logic.** All business logic, EF Core queries, and AI calls go in `Services/`. Register services in `Extensions/` using `IServiceCollection` extension methods.
- **Models are EF Core entities only.** Do not put business logic in model classes.
- **DTOs are separate.** All request and response shapes go in `DTOs/`. Naming convention: `XxxDto`, `XxxRequestDto`, `XxxResponseDto`.

## Patterns to Follow

```csharp
// Controller pattern
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher")]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _service;
    private readonly ILogger<ExampleController> _logger;

    public ExampleController(IExampleService service, ILogger<ExampleController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExampleResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
```

## Entity Framework Core

- Use `Guid` as primary key for all entities.
- Configure table names in snake_case inside `OnModelCreating` (e.g., `"test_submissions"`).
- Store list/dictionary columns as JSONB using `[Column(TypeName = "jsonb")]`.
- Always include navigation properties and configure relationships explicitly.
- Migrations go in `Migrations/` — run `dotnet ef migrations add <Name>` from the backend project folder.

## Configuration & Secrets

- Read configuration via injected `IConfiguration` or a strongly-typed options class in `Configuration/`.
- Never hardcode connection strings, JWT secrets, or AI API keys in source files.
- Development secrets go in `appsettings.Development.json` (gitignored).

## Authentication & Authorization

- Use `[Authorize]` on every endpoint that requires a logged-in user.
- Specify roles explicitly: `[Authorize(Roles = "Admin")]`, `[Authorize(Roles = "Teacher,Admin")]`, etc.
- Current roles are: `Admin`, `Teacher`, `Student`.
- Read the current user's ID from `User.FindFirstValue(ClaimTypes.NameIdentifier)`.

## Error Handling & Logging

- Log errors with `_logger.LogError(ex, "Descriptive message with {Parameter}", value)`.
- Return appropriate HTTP status codes: `404 NotFound`, `400 BadRequest`, `403 Forbidden`, `500` only for unhandled exceptions.
- Never swallow exceptions silently.

## Async

- All service methods and controller actions must be `async Task<T>`.
- Never use `.Result`, `.Wait()`, or `Task.Run()` for I/O operations.

## XML Comments

- Add XML doc comments (`/// <summary>`) to all public service interfaces and DTO classes.
