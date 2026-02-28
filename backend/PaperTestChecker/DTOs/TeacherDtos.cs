using System.ComponentModel.DataAnnotations;

namespace PaperTestChecker.DTOs;

// Request: generate a test from selected question IDs
public record GenerateTestDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [Required]
    public Guid StudentId { get; init; }

    [Required, MinLength(1)]
    public List<Guid> QuestionResultIds { get; init; } = [];
}

// Response: student info for teacher view
public record StudentInfoDto(
    Guid Id,
    string Name,
    string Email,
    int SubmissionCount);

// Response: question from student's history (for teacher to select)
public record StudentQuestionDto(
    Guid Id,
    Guid SubmissionId,
    int QuestionNumber,
    string QuestionText,
    string CorrectAnswer,
    string StudentAnswer,
    bool IsCorrect,
    string[] Options,
    DateTime SubmittedAt);

// Response: generated test
public record GeneratedTestDto(
    Guid Id,
    string Title,
    string StudentName,
    Guid ForStudentUserId,
    DateTime CreatedAt,
    List<GeneratedTestItemDto> Items);

public record GeneratedTestItemDto(
    int QuestionNumber,
    string QuestionText,
    string[] Options,
    string CorrectAnswer);

// Response: test list summary
public record GeneratedTestSummaryDto(
    Guid Id,
    string Title,
    string StudentName,
    int QuestionCount,
    int AttemptCount,
    DateTime CreatedAt);

// Teacher view of a test attempt
public record TeacherTestAttemptDto(
    Guid AttemptId,
    string TestTitle,
    string StudentName,
    int Score,
    int MaxScore,
    DateTime CompletedAt);

// Detailed attempt view
public record TestAttemptDetailDto(
    Guid AttemptId,
    string TestTitle,
    string StudentName,
    int Score,
    int MaxScore,
    DateTime CompletedAt,
    List<TestAttemptAnswerDetailDto> Answers);

public record TestAttemptAnswerDetailDto(
    int QuestionNumber,
    string QuestionText,
    string SelectedAnswer,
    string CorrectAnswer,
    bool IsCorrect);

