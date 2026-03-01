using System.Net;
using System.Net.Http.Json;
using PaperTestChecker.DTOs;

namespace PaperTestChecker.Tests.Integration;

/// <summary>
/// Integration tests for AuthController.
/// Each test creates its own dedicated HttpClient to ensure a fresh database state.
/// </summary>
public class AuthControllerTests
{
    private static HttpClient CreateClient() => new IntegrationWebAppFactory().CreateClient();

    [Fact]
    public async Task Register_ValidRequest_Returns201Created()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Alice",
            email = "alice@test.com",
            password = "password123",
            role = "student"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Token);
        Assert.Equal("student", body.Role);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409Conflict()
    {
        var client = CreateClient();
        var payload = new { name = "Bob", email = "bob@test.com", password = "pass1", role = "student" };
        await client.PostAsJsonAsync("/api/auth/register", payload);

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Carol",
            email = "carol@test.com",
            password = "mypassword",
            role = "student"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "carol@test.com",
            password = "mypassword"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Token);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401Unauthorized()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Dave",
            email = "dave@test.com",
            password = "correctpass",
            role = "student"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "dave@test.com",
            password = "WRONGPASSWORD"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentUser_Returns401Unauthorized()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "nonexistent@test.com",
            password = "somepassword"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
