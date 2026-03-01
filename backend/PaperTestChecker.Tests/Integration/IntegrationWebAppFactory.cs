using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaperTestChecker.Data;

namespace PaperTestChecker.Tests.Integration;

/// <summary>
/// A WebApplicationFactory that replaces the real Postgres DB with an in-memory EF Core database,
/// overrides JWT settings, and disables HTTPS redirect so the test HttpClient can communicate
/// over plain HTTP.
/// </summary>
public class IntegrationWebAppFactory : WebApplicationFactory<Program>
{
    public const string TestJwtSecret = "SuperSecretTestKeyForIntegrationTests32Chars!";
    public const string TestJwtIssuer = "Test";
    public const string TestJwtAudience = "Test";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 1. Override configuration before the app builds
        builder.UseSetting("Jwt:Secret", TestJwtSecret);
        builder.UseSetting("Jwt:Issuer", TestJwtIssuer);
        builder.UseSetting("Jwt:Audience", TestJwtAudience);
        builder.UseSetting("AllowedOrigins", "");
        builder.UseSetting("Ai:ApiKey", "test-key");
        // Run as Development so UseHttpsRedirection is skipped
        builder.UseEnvironment("Development");
        // Provide a dummy connection string so EF startup doesn't throw before we replace it
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=test_placeholder");

        // 2. Replace the DB after the main app registers services (ConfigureTestServices runs last)
        builder.ConfigureTestServices(services =>
        {
            // Remove the Postgres DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Also remove the AppDbContext registration itself
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AppDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            // Add in-memory DB
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        });
    }
}

