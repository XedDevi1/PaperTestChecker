using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaperTestChecker.Configuration;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Services;

namespace PaperTestChecker.Tests.Unit;

public class AuthServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static AuthService CreateService(AppDbContext db)
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "SuperSecretTestKeyThatIsLongEnough32!",
            Issuer = "Test",
            Audience = "Test",
            ExpirationDays = 1
        });
        return new AuthService(db, jwtSettings);
    }

    private static UserRegisterDto MakeRegister(string name, string email, string password = "password123", string role = "student")
        => new() { Name = name, Email = email, Password = password, Role = role };

    private static UserLoginDto MakeLogin(string email, string password)
        => new() { Email = email, Password = password };

    [Fact]
    public async Task Register_NewUser_ReturnsTokenAndCorrectRole()
    {
        var db = CreateDb();
        var service = CreateService(db);

        var result = await service.RegisterAsync(MakeRegister("Alice", "alice@test.com"));

        Assert.NotNull(result);
        Assert.Equal("student", result.Role);
        Assert.Equal("alice@test.com", result.Email);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Register_DefaultsToStudentRole_WhenRoleIsEmpty()
    {
        var db = CreateDb();
        var service = CreateService(db);

        var result = await service.RegisterAsync(MakeRegister("Bob", "bob@test.com", role: ""));

        Assert.NotNull(result);
        Assert.Equal("student", result.Role);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsNull()
    {
        var db = CreateDb();
        var service = CreateService(db);
        await service.RegisterAsync(MakeRegister("Alice", "alice@test.com"));

        var result = await service.RegisterAsync(MakeRegister("Alice2", "alice@test.com"));

        Assert.Null(result);
    }

    [Fact]
    public async Task Register_InvalidRole_ThrowsArgumentException()
    {
        var db = CreateDb();
        var service = CreateService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.RegisterAsync(MakeRegister("Charlie", "c@test.com", role: "hacker")));
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var db = CreateDb();
        var service = CreateService(db);
        await service.RegisterAsync(MakeRegister("Alice", "alice@test.com", "correctpass"));

        var result = await service.LoginAsync(MakeLogin("alice@test.com", "correctpass"));

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("alice@test.com", result.Email);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var db = CreateDb();
        var service = CreateService(db);
        await service.RegisterAsync(MakeRegister("Alice", "alice@test.com", "correctpass"));

        var result = await service.LoginAsync(MakeLogin("alice@test.com", "WRONGPASSWORD"));

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_NonExistentEmail_ReturnsNull()
    {
        var db = CreateDb();
        var service = CreateService(db);

        var result = await service.LoginAsync(MakeLogin("nobody@test.com", "pass"));

        Assert.Null(result);
    }
}
