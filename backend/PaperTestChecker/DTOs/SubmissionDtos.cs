namespace PaperTestChecker.DTOs;

public record SubmissionResponseDto(
    Guid Id,
    Guid UserId,
    int TotalScore,
    int MaxScore,
    DateTime CreatedAt,
    List<QuestionResultDto> Questions);

public record QuestionResultDto(
    int QuestionNumber,
    string QuestionText,
    string StudentAnswer,
    string CorrectAnswer,
    bool IsCorrect,
    string Feedback,
    string[] RecommendedReadings);

public record SubmissionSummaryDto(
    Guid Id,
    int TotalScore,
    int MaxScore,
    DateTime CreatedAt);
