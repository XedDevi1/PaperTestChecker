using System.ComponentModel.DataAnnotations;
using PaperTestChecker.Configuration;

namespace PaperTestChecker.DTOs;

public record UserRegisterDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;

    [AllowedValues(AppRoles.Admin, AppRoles.Teacher, AppRoles.Student)]
    public string Role { get; init; } = AppRoles.Student;
}

public record UserLoginDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public record AuthResponseDto(string Token, string Role, string Email, string Name);
public record UserInfoDto(Guid Id, string Name, string Email, string Role, DateTime CreatedAt);
