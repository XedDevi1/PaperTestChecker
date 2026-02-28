using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Models;

namespace PaperTestChecker.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;

    public SubmissionService(AppDbContext db, IGeminiService gemini)
    {
        _db = db;
        _gemini = gemini;
    }

    public async Task<SubmissionResponseDto> AnalyzeTestPhotoAsync(
        Guid userId, Stream imageStream, string fileName, string contentType)
    {
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var analysis = await _gemini.AnalyzeTestImageAsync(imageBytes, contentType);

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ImageFileName = fileName,
            TotalScore = analysis.TotalScore,
            MaxScore = analysis.MaxScore,
            AnalysisJson = JsonSerializer.Serialize(analysis),
            CreatedAt = DateTime.UtcNow,
            QuestionResults = analysis.Questions.Select(q => new QuestionResult
            {
                Id = Guid.NewGuid(),
                QuestionNumber = q.QuestionNumber,
                QuestionText = q.QuestionText,
                StudentAnswer = q.StudentAnswer,
                CorrectAnswer = q.CorrectAnswer,
                IsCorrect = q.IsCorrect,
                Feedback = q.Feedback,
                RecommendedReadings = q.RecommendedReadings,
                Options = q.Options ?? []
            }).ToList()
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();

        return MapToResponse(submission);
    }

    public async Task<SubmissionResponseDto?> GetSubmissionAsync(Guid id, Guid userId)
    {
        var submission = await _db.Submissions
            .Include(s => s.QuestionResults.OrderBy(q => q.QuestionNumber))
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        return submission is null ? null : MapToResponse(submission);
    }

    public async Task<List<SubmissionSummaryDto>> GetUserSubmissionsAsync(Guid userId)
    {
        return await _db.Submissions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubmissionSummaryDto(s.Id, s.TotalScore, s.MaxScore, s.CreatedAt))
            .ToListAsync();
    }

    private static SubmissionResponseDto MapToResponse(Submission s)
    {
        return new SubmissionResponseDto(
            s.Id, s.UserId, s.TotalScore, s.MaxScore, s.CreatedAt,
            s.QuestionResults
                .OrderBy(q => q.QuestionNumber)
                .Select(q => new QuestionResultDto(
                    q.QuestionNumber, q.QuestionText, q.StudentAnswer,
                    q.CorrectAnswer, q.IsCorrect, q.Feedback,
                    q.RecommendedReadings))
                .ToList());
    }
}
