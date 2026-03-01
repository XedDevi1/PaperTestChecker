using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PaperTestChecker.DTOs;

namespace PaperTestChecker.Tests.Integration;

/// <summary>
/// Integration tests for StudentTestsController.
/// Each test uses its own fresh HttpClient and DB.
/// </summary>
public class StudentTestsControllerTests
{
    private static HttpClient CreateClient() => new IntegrationWebAppFactory().CreateClient();

    private static async Task<(HttpClient client, string token)> RegisterAndLoginAsync()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Student",
            email = "student@test.com",
            password = "password123",
            role = "student"
        });
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "student@test.com",
            password = "password123"
        });
        var body = await loginResp.Content.ReadFromJsonAsync<AuthResponseDto>();
        return (client, body!.Token);
    }

    [Fact]
    public async Task GetTests_WithoutToken_Returns401()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/student/tests");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTests_WithValidToken_Returns200AndEmptyList()
    {
        var (client, token) = await RegisterAndLoginAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/student/tests");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tests = await response.Content.ReadFromJsonAsync<List<StudentTestSummaryDto>>();
        Assert.NotNull(tests);
        Assert.Empty(tests);
    }
}
