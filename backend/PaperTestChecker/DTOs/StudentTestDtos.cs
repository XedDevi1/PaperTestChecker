using System.ComponentModel.DataAnnotations;

namespace PaperTestChecker.DTOs;

// Student view of assigned test (no correct answers!)
public record StudentTestSummaryDto(
    Guid Id,
    string Title,
    string TeacherName,
    int QuestionCount,
    DateTime CreatedAt,
    bool AlreadyTaken);

// Test to take (questions with options, no correct answer)
public record StudentTestDto(
    Guid Id,
    string Title,
    string TeacherName,
    List<StudentTestQuestionDto> Questions);

public record StudentTestQuestionDto(
    int QuestionNumber,
    string QuestionText,
    string[] Options);

// Submit answers
public record SubmitTestDto
{
    [Required, MinLength(1)]
    public List<SubmitAnswerDto> Answers { get; init; } = [];
}

public record SubmitAnswerDto
{
    [Required]
    public int QuestionNumber { get; init; }

    [Required]
    public string SelectedAnswer { get; init; } = string.Empty;
}

// Result after submission
public record TestAttemptResultDto(
    Guid Id,
    string TestTitle,
    int Score,
    int MaxScore,
    DateTime CompletedAt,
    List<TestAttemptAnswerDto> Answers);

public record TestAttemptAnswerDto(
    int QuestionNumber,
    string QuestionText,
    string SelectedAnswer,
    string CorrectAnswer,
    bool IsCorrect);
