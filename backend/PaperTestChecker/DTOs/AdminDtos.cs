using System.ComponentModel.DataAnnotations;

namespace PaperTestChecker.DTOs;

// Admin statistics
public record AdminStatsDto(
    int TotalUsers,
    int StudentCount,
    int TeacherCount,
    int AdminCount,
    int TotalSubmissions,
    int TotalGeneratedTests,
    int TotalTestAttempts);

// Admin change password
public record ChangePasswordDto
{
    [Required]
    public Guid UserId { get; init; }

    [Required, MinLength(6)]
    public string NewPassword { get; init; } = string.Empty;
}

// Admin view of user with extra details
public record AdminUserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    DateTime CreatedAt,
    int SubmissionCount,
    int TestAttemptCount);
