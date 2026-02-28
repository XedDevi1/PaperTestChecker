using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Models;

namespace PaperTestChecker.Services;

public class TeacherService : ITeacherService
{
    private readonly AppDbContext _db;

    public TeacherService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StudentInfoDto>> GetStudentsAsync()
    {
        var students = await _db.Users
            .Where(u => u.Role == "student")
            .OrderBy(u => u.Name)
            .ToListAsync();

        var submissionCounts = await _db.Submissions
            .GroupBy(s => s.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        return students.Select(u => new StudentInfoDto(
            u.Id,
            u.Name,
            u.Email,
            submissionCounts.GetValueOrDefault(u.Id, 0)
        )).ToList();
    }

    public async Task<List<StudentQuestionDto>> GetStudentQuestionsAsync(Guid studentId)
    {
        return await _db.QuestionResults
            .Where(q => q.Submission.UserId == studentId)
            .OrderByDescending(q => q.Submission.CreatedAt)
            .ThenBy(q => q.QuestionNumber)
            .Select(q => new StudentQuestionDto(
                q.Id,
                q.SubmissionId,
                q.QuestionNumber,
                q.QuestionText,
                q.CorrectAnswer,
                q.StudentAnswer,
                q.IsCorrect,
                q.Options,
                q.Submission.CreatedAt))
            .ToListAsync();
    }

    public async Task<GeneratedTestDto> GenerateTestAsync(Guid teacherId, GenerateTestDto dto)
    {
        var questions = await _db.QuestionResults
            .Where(q => dto.QuestionResultIds.Contains(q.Id) && q.Submission.UserId == dto.StudentId)
            .ToListAsync();

        if (questions.Count == 0)
            throw new InvalidOperationException("No valid questions found for the given IDs");

        var student = await _db.Users.FindAsync(dto.StudentId)
            ?? throw new InvalidOperationException("Student not found");

        var test = new GeneratedTest
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            CreatedByUserId = teacherId,
            ForStudentUserId = dto.StudentId,
            CreatedAt = DateTime.UtcNow,
            Items = questions.Select((q, i) => new GeneratedTestItem
            {
                Id = Guid.NewGuid(),
                QuestionNumber = i + 1,
                QuestionText = q.QuestionText,
                Options = q.Options.Length > 0 ? Shuffle(q.Options) : [q.CorrectAnswer],
                CorrectAnswer = q.CorrectAnswer,
                SourceQuestionResultId = q.Id
            }).ToList()
        };

        _db.GeneratedTests.Add(test);
        await _db.SaveChangesAsync();

        return MapToDto(test, student.Name);
    }

    public async Task<List<GeneratedTestSummaryDto>> GetGeneratedTestsAsync(Guid teacherId)
    {
        return await _db.GeneratedTests
            .Where(t => t.CreatedByUserId == teacherId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new GeneratedTestSummaryDto(
                t.Id,
                t.Title,
                t.ForStudentUser.Name,
                t.Items.Count,
                _db.TestAttempts.Count(a => a.GeneratedTestId == t.Id),
                t.CreatedAt))
            .ToListAsync();
    }

    public async Task<GeneratedTestDto?> GetGeneratedTestAsync(Guid testId)
    {
        var test = await _db.GeneratedTests
            .Include(t => t.ForStudentUser)
            .Include(t => t.Items.OrderBy(i => i.QuestionNumber))
            .FirstOrDefaultAsync(t => t.Id == testId);

        return test is null ? null : MapToDto(test, test.ForStudentUser.Name);
    }

    private static GeneratedTestDto MapToDto(GeneratedTest t, string studentName)
    {
        return new GeneratedTestDto(
            t.Id, t.Title, studentName, t.ForStudentUserId, t.CreatedAt,
            t.Items.OrderBy(i => i.QuestionNumber).Select(i => new GeneratedTestItemDto(
                i.QuestionNumber, i.QuestionText, i.Options, i.CorrectAnswer
            )).ToList());
    }

    private static string[] Shuffle(string[] array)
    {
        var rng = Random.Shared;
        var copy = array.ToArray();
        for (int i = copy.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }
        return copy;
    }

    public async Task<List<TeacherTestAttemptDto>> GetTestAttemptsAsync(Guid teacherId)
    {
        return await _db.TestAttempts
            .Where(a => a.GeneratedTest.CreatedByUserId == teacherId)
            .OrderByDescending(a => a.CompletedAt)
            .Select(a => new TeacherTestAttemptDto(
                a.Id,
                a.GeneratedTest.Title,
                a.Student.Name,
                a.Score,
                a.MaxScore,
                a.CompletedAt))
            .ToListAsync();
    }

    public async Task<TestAttemptDetailDto?> GetTestAttemptDetailAsync(Guid attemptId)
    {
        var attempt = await _db.TestAttempts
            .Include(a => a.GeneratedTest)
                .ThenInclude(t => t.Items)
            .Include(a => a.Student)
            .Include(a => a.Answers.OrderBy(ans => ans.QuestionNumber))
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        if (attempt is null) return null;

        var itemsByNumber = attempt.GeneratedTest.Items.ToDictionary(i => i.QuestionNumber);

        return new TestAttemptDetailDto(
            attempt.Id,
            attempt.GeneratedTest.Title,
            attempt.Student.Name,
            attempt.Score,
            attempt.MaxScore,
            attempt.CompletedAt,
            attempt.Answers.OrderBy(a => a.QuestionNumber).Select(a => new TestAttemptAnswerDetailDto(
                a.QuestionNumber,
                itemsByNumber.GetValueOrDefault(a.QuestionNumber)?.QuestionText ?? "",
                a.SelectedAnswer,
                a.CorrectAnswer,
                a.IsCorrect
            )).ToList());
    }
}
