using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Configuration;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Models;

namespace PaperTestChecker.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var users = await _db.Users.ToListAsync();
        var totalSubmissions = await _db.Submissions.CountAsync();
        var totalTests = await _db.GeneratedTests.CountAsync();
        var totalAttempts = await _db.TestAttempts.CountAsync();

        return new AdminStatsDto(
            TotalUsers: users.Count,
            StudentCount: users.Count(u => u.Role == "student"),
            TeacherCount: users.Count(u => u.Role == "teacher"),
            AdminCount: users.Count(u => u.Role == "admin"),
            TotalSubmissions: totalSubmissions,
            TotalGeneratedTests: totalTests,
            TotalTestAttempts: totalAttempts);
    }

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
    {
        var users = await _db.Users
            .OrderBy(u => u.Name)
            .ToListAsync();

        var submissionCounts = await _db.Submissions
            .GroupBy(s => s.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var attemptCounts = await _db.TestAttempts
            .GroupBy(a => a.StudentUserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        return users.Select(u => new AdminUserDto(
            u.Id, u.Name, u.Email, u.Role, u.CreatedAt,
            submissionCounts.GetValueOrDefault(u.Id, 0),
            attemptCounts.GetValueOrDefault(u.Id, 0)
        )).ToList();
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return false;

        // Delete related data
        var submissions = await _db.Submissions.Where(s => s.UserId == userId).ToListAsync();
        _db.Submissions.RemoveRange(submissions);

        var attempts = await _db.TestAttempts.Where(a => a.StudentUserId == userId).ToListAsync();
        _db.TestAttempts.RemoveRange(attempts);

        // Delete generated tests by this user (if teacher)
        var tests = await _db.GeneratedTests
            .Include(t => t.Items)
            .Where(t => t.CreatedByUserId == userId || t.ForStudentUserId == userId)
            .ToListAsync();

        foreach (var test in tests)
        {
            var testAttempts = await _db.TestAttempts.Where(a => a.GeneratedTestId == test.Id).ToListAsync();
            _db.TestAttempts.RemoveRange(testAttempts);
        }
        _db.GeneratedTests.RemoveRange(tests);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await _db.Users.FindAsync(dto.UserId);
        if (user is null) return false;

        user.PasswordHash = _hasher.HashPassword(user, dto.NewPassword);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeRoleAsync(Guid userId, string role)
    {
        if (role is not (AppRoles.Admin or AppRoles.Teacher or AppRoles.Student))
            return false;

        var user = await _db.Users.FindAsync(userId);
        if (user is null) return false;

        user.Role = role;
        await _db.SaveChangesAsync();
        return true;
    }
}
