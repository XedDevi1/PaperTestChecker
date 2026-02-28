using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Models;

namespace PaperTestChecker.Services;

public class StudentTestService : IStudentTestService
{
    private readonly AppDbContext _db;

    public StudentTestService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StudentTestSummaryDto>> GetTestsForStudentAsync(Guid studentId)
    {
        var tests = await _db.GeneratedTests
            .Where(t => t.ForStudentUserId == studentId)
            .Include(t => t.CreatedByUser)
            .Include(t => t.Items)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var attemptedTestIds = await _db.TestAttempts
            .Where(a => a.StudentUserId == studentId)
            .Select(a => a.GeneratedTestId)
            .ToListAsync();

        return tests.Select(t => new StudentTestSummaryDto(
            t.Id,
            t.Title,
            t.CreatedByUser.Name,
            t.Items.Count,
            t.CreatedAt,
            attemptedTestIds.Contains(t.Id)
        )).ToList();
    }

    public async Task<StudentTestDto?> GetTestForTakingAsync(Guid testId, Guid studentId)
    {
        var test = await _db.GeneratedTests
            .Where(t => t.Id == testId && t.ForStudentUserId == studentId)
            .Include(t => t.CreatedByUser)
            .Include(t => t.Items.OrderBy(i => i.QuestionNumber))
            .FirstOrDefaultAsync();

        if (test is null) return null;

        return new StudentTestDto(
            test.Id,
            test.Title,
            test.CreatedByUser.Name,
            test.Items.Select(i => new StudentTestQuestionDto(
                i.QuestionNumber,
                i.QuestionText,
                i.Options
            )).ToList()
        );
    }

    public async Task<TestAttemptResultDto> SubmitTestAsync(Guid testId, Guid studentId, SubmitTestDto dto)
    {
        var test = await _db.GeneratedTests
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == testId && t.ForStudentUserId == studentId)
            ?? throw new InvalidOperationException("Test not found");

        // Check if already taken
        var alreadyTaken = await _db.TestAttempts
            .AnyAsync(a => a.GeneratedTestId == testId && a.StudentUserId == studentId);
        if (alreadyTaken)
            throw new InvalidOperationException("You have already taken this test");

        var itemsByNumber = test.Items.ToDictionary(i => i.QuestionNumber);

        var answers = dto.Answers.Select(a =>
        {
            var item = itemsByNumber.GetValueOrDefault(a.QuestionNumber);
            var correctAnswer = item?.CorrectAnswer ?? "";
            return new TestAttemptAnswer
            {
                Id = Guid.NewGuid(),
                QuestionNumber = a.QuestionNumber,
                SelectedAnswer = a.SelectedAnswer,
                CorrectAnswer = correctAnswer,
                IsCorrect = string.Equals(a.SelectedAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase)
            };
        }).ToList();

        var attempt = new TestAttempt
        {
            Id = Guid.NewGuid(),
            GeneratedTestId = testId,
            StudentUserId = studentId,
            Score = answers.Count(a => a.IsCorrect),
            MaxScore = test.Items.Count,
            CompletedAt = DateTime.UtcNow,
            Answers = answers
        };

        _db.TestAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return MapToResult(attempt, test.Title, test.Items);
    }

    public async Task<TestAttemptResultDto?> GetAttemptAsync(Guid attemptId, Guid studentId)
    {
        var attempt = await _db.TestAttempts
            .Where(a => a.Id == attemptId && a.StudentUserId == studentId)
            .Include(a => a.GeneratedTest)
                .ThenInclude(t => t.Items)
            .Include(a => a.Answers.OrderBy(ans => ans.QuestionNumber))
            .FirstOrDefaultAsync();

        if (attempt is null) return null;

        return MapToResult(attempt, attempt.GeneratedTest.Title, attempt.GeneratedTest.Items);
    }

    private static TestAttemptResultDto MapToResult(TestAttempt attempt, string title, List<GeneratedTestItem> items)
    {
        var itemsByNumber = items.ToDictionary(i => i.QuestionNumber);

        return new TestAttemptResultDto(
            attempt.Id,
            title,
            attempt.Score,
            attempt.MaxScore,
            attempt.CompletedAt,
            attempt.Answers.OrderBy(a => a.QuestionNumber).Select(a => new TestAttemptAnswerDto(
                a.QuestionNumber,
                itemsByNumber.GetValueOrDefault(a.QuestionNumber)?.QuestionText ?? "",
                a.SelectedAnswer,
                a.CorrectAnswer,
                a.IsCorrect
            )).ToList()
        );
    }
}
